using System;
using System.IO;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashVariableDefinitionStatementTranspiler : VariableDefinitionStatementTranspilerBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteVariableDefinition(Context context, Scope scope, TextWriter writer, string name,
            string expression)
        {
            if (scope.IsRootScope)
                writer.WriteLine($"{name}={expression}");
            else
                writer.WriteLine($"local {name}={expression}");
        }


        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            if (varDefStt.IsConstant)
            {
                if (!varDefStt.HasDefaultValue)
                {
                    throw new InvalidOperationException();
                }

                var result =
                    EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, varDefStt.DefaultValue);

                if (!(result is ConstantValueStatement constantValueStatement))
                {
                    throw new InvalidOperationException(ErrorMessages.ConstantValueRequired);
                }

                scope.ReserveNewConstant(varDefStt.DataType, varDefStt.Name, constantValueStatement.Value);
            }
            else
            {
                if (varDefStt.HasDefaultValue)
                {
                    var def = varDefStt.DefaultValue;
                    var transpiler = context.GetEvaluationTranspilerForStatement(def);
                    if (transpiler == null)
                    {
                        throw new InvalidOperationException();
                    }

                    //it pins the non-inlineable values to a helper variable:
                    //int x = 34 * myFunc();
                    //becomes:
                    //myFuncResult=myFunc()
                    //x=$((34 * myFuncResult))
                    var (dataType, expression) = transpiler.GetInline(context, scope, metaWriter, writer, null, def);

                    WriteVariableDefinition(context, scope, writer, varDefStt.Name, expression);
                }
                else
                {
                    WriteVariableDefinition(context, scope, writer, varDefStt.Name,
                        DesignGuidelines.GetDefaultValue(varDefStt.DataType));
                }

                scope.ReserveNewVariable(varDefStt.DataType, varDefStt.Name);
            }
        }
    }
}