using System;
using System.IO;
using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashVariableDefinitionStatementTranspiler : VariableDefinitionStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement)
        {
            if (!(statement is VariableDefinitionStatement varDefStt)) throw new InvalidOperationException();

            if (varDefStt.IsConstant)
            {
                if (!varDefStt.HasDefaultValue)
                {
                    throw new InvalidOperationException();
                }

                var result =
                    EvaluationStatementTranspilerBase.CalculateEvaluation(context, scope, varDefStt.DefaultValue);

                if (!(result is ConstantValueStatement constantValueStatement))
                {
                    throw new InvalidOperationException(ErrorMessages.ConstantValueRequired);
                }

                scope.ReserveNewConstant(varDefStt.DataType, varDefStt.Name, constantValueStatement.Value);
            }
            else
            {
                scope.ReserveNewVariable(varDefStt.DataType, varDefStt.Name);

                if (varDefStt.HasDefaultValue)
                {
                    var def = varDefStt.DefaultValue;
                    var transpiler = context.GetEvaluationTranspilerForStatement(def);
                    if (transpiler == null)
                    {
                        throw new InvalidOperationException();
                    }

                    string defaultValue;
                    //it pins the non-inlineable values to a helper variable:
                    //int x = 34 * myFunc();
                    //becomes:
                    //myFuncResult=myFunc()
                    //x=$((34 * myFuncResult))
                    using (var textWriter = new StringWriter())
                    {
                        transpiler.WriteInline(context, scope, textWriter, writer, def);

                        defaultValue = textWriter.ToString();
                    }

                    if (scope.IsRootScope)
                        writer.WriteLine($"{varDefStt.Name}={defaultValue}");
                    else
                        writer.WriteLine($"local {varDefStt.Name}={defaultValue}");
                }
                else
                {
                    writer.WriteLine($"{varDefStt.Name}={DesignGuidelines.GetDefaultValue(varDefStt.DataType)}");
                }
            }
        }
    }
}