using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    internal static class BashStringBuilderHelpers
    {
        public static string RequireBuilderVariable(ExpressionBuilderParams p, EvaluationStatement parameter)
        {
            if (!(parameter is VariableAccessStatement builder))
            {
                throw new InvalidStatementStructureCompilerException(parameter, parameter.Info);
            }

            if (!p.Scope.TryGetVariableInfo(builder, out var builderInfo))
            {
                throw new IdentifierNotFoundCompilerException(builder);
            }

            return builderInfo.AccessName;
        }

        public static string GetParameterExpression(ExpressionBuilderParams p, FunctionCallStatement call,
            EvaluationStatement parameter)
        {
            var usage = p.UsageContext ?? call;
            var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
            return transpiler
                .GetExpression(p.Context, p.Scope, p.MetaWriter, p.NonInlinePartWriter, usage, parameter)
                .Expression;
        }

        public static IApiMethodBuilderResult VoidMutationResult() =>
            new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor.Void, null, null,
                ExpressionBuilderBase.PinRequiredNotice));
    }
}
