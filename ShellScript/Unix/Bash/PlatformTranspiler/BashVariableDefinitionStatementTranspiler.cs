using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string WriteLastStatusCodeStoreVariableDefinition(Context context, Scope scope, TextWriter writer,
            string nameHint)
        {
            var name = scope.NewHelperVariable(TypeDescriptor.Integer, nameHint);

            if (scope.IsRootScope)
                writer.WriteLine($"{name}=$?");
            else
                writer.WriteLine($"local {name}=$?");

            return name;
        }


        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
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

                if (!StatementHelpers.IsAssignableFrom(context, scope, varDefStt.TypeDescriptor,
                    constantValueStatement.TypeDescriptor))
                {
                    throw new TypeMismatchCompilerException(constantValueStatement.TypeDescriptor,
                        varDefStt.TypeDescriptor, varDefStt.Info);
                }

                scope.ReserveNewConstant(varDefStt.TypeDescriptor, varDefStt.Name, constantValueStatement.Value);
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

                    //it pins non-inlinable values to a helper variable:
                    //int x = 34 * myFunc();
                    //becomes:
                    //myFuncResult=myFunc()
                    //x=$((34 * myFuncResult))
                    var result = transpiler.GetExpression(context, scope, metaWriter, writer, null, def);

                    if (!StatementHelpers.IsAssignableFrom(context, scope, varDefStt.TypeDescriptor,
                        result.TypeDescriptor))
                    {
                        throw new TypeMismatchCompilerException(result.TypeDescriptor, varDefStt.TypeDescriptor,
                            varDefStt.Info);
                    }

                    WriteVariableDefinition(context, scope, writer, varDefStt.Name, result.Expression);
                }
                else
                {
                    WriteVariableDefinition(context, scope, writer, varDefStt.Name,
                        context.Platform.GetDefaultValue(varDefStt.TypeDescriptor.DataType));
                }

                scope.ReserveNewVariable(varDefStt.TypeDescriptor, varDefStt.Name);

                scope.IncrementStatements();
            }
        }
    }
}