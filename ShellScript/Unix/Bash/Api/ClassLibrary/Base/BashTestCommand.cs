using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public abstract class BashTestCommand
    {
        public delegate ExpressionResult TestExpressionCreator(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement);

        public static IApiMethodBuilderResult CreateTestExpression(ApiBaseFunction func, ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement, string testChar)
        {
            return CreateTestExpression(func, p, functionCallStatement, (parameters, call) =>
            {
                var parameter = call.Parameters[0];
                var transpiler = parameters.Context.GetEvaluationTranspilerForStatement(parameter);
                var result = transpiler.GetExpression(parameters.Context, parameters.Scope, parameters.MetaWriter,
                    parameters.NonInlinePartWriter, call, parameter);
                return new ExpressionResult(func.TypeDescriptor, $"[ -{testChar} {result.Expression} ]", call);
            });
        }

        public static IApiMethodBuilderResult CreateTestExpression(ApiBaseFunction func, ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement, TestExpressionCreator createTestExpression)
        {
            func.AssertParameters(p, functionCallStatement.Parameters);
            var result = createTestExpression(p, functionCallStatement);

            if (p.UsageContext is IfElseStatement || p.UsageContext is ConditionalBlockStatement)
                return new ApiMethodBuilderRawResult(result);

            var variableName = p.Scope.NewHelperVariable(TypeDescriptor.Boolean,
                $"{functionCallStatement.Fqn}_Result");
            p.NonInlinePartWriter.Write("if ");
            p.NonInlinePartWriter.Write(result.Expression);
            p.NonInlinePartWriter.WriteLine("; then");
            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context, p.Scope, p.NonInlinePartWriter, variableName, "1");
            p.NonInlinePartWriter.WriteLine("else");
            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context, p.Scope, p.NonInlinePartWriter, variableName, "0");
            p.NonInlinePartWriter.WriteLine("fi");

            return new ApiMethodBuilderRawResult(new ExpressionResult(
                func.TypeDescriptor, $"${variableName}",
                new VariableAccessStatement(variableName, functionCallStatement.Info),
                ExpressionBuilderBase.PinRequiredNotice));
        }
    }
}
