using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    internal static class BashArrayArgumentHelpers
    {
        public static string ResolveArrayVariableName(ExpressionBuilderParams p, EvaluationStatement parameter,
            FunctionCallStatement call)
        {
            if (parameter is VariableAccessStatement variableAccess)
            {
                if (!p.Scope.TryGetVariableInfo(variableAccess, out var variableInfo))
                {
                    throw new IdentifierNotFoundCompilerException(variableAccess);
                }

                if (!variableInfo.TypeDescriptor.IsArray())
                {
                    throw new TypeMismatchCompilerException(variableInfo.TypeDescriptor, TypeDescriptor.Array,
                        variableAccess.Info);
                }

                return variableInfo.AccessName;
            }

            if (parameter is ArrayStatement arrayStatement)
            {
                string sourceName;

                if (arrayStatement.ParentStatement is VariableDefinitionStatement variableDefinitionStatement)
                {
                    sourceName = variableDefinitionStatement.Name;
                }
                else if (arrayStatement.ParentStatement is AssignmentStatement assignmentStatement)
                {
                    if (!(assignmentStatement.LeftSide is VariableAccessStatement left))
                    {
                        throw new InvalidStatementStructureCompilerException(assignmentStatement,
                            assignmentStatement.Info);
                    }

                    sourceName = left.VariableName;
                }
                else if (arrayStatement.ParentStatement is ReturnStatement)
                {
                    sourceName = p.Context.GetLastFunctionCallStorageVariable(arrayStatement.Type, p.MetaWriter);
                }
                else
                {
                    sourceName = p.Scope.NewHelperVariable(arrayStatement.Type, "array_helper");
                }

                if (arrayStatement.Elements == null)
                {
                    throw new InvalidStatementStructureCompilerException(arrayStatement, arrayStatement.Info);
                }

                for (var i = 0; i < arrayStatement.Elements.Length; i++)
                {
                    p.NonInlinePartWriter.Write(sourceName);
                    p.NonInlinePartWriter.Write('[');
                    p.NonInlinePartWriter.Write(i);
                    p.NonInlinePartWriter.Write("]=");

                    var element = BashStringBuilderHelpers.GetParameterExpression(p, call, arrayStatement.Elements[i]);
                    p.NonInlinePartWriter.WriteLine(element);
                }

                return sourceName;
            }

            throw new InvalidStatementStructureCompilerException(parameter, parameter.Info);
        }
    }
}
