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
            func.AssertParameters(p, functionCallStatement.Parameters);

            var parameter = functionCallStatement.Parameters[0];

            var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
            var result = transpiler.GetExpression(p.Context, p.Scope, p.MetaWriter, p.NonInlinePartWriter, null,
                parameter);

            result = new ExpressionResult(
                func.TypeDescriptor,
                $"[ -{testChar} {result.Expression} ]",
                functionCallStatement
            );

            if (p.UsageContext is IfElseStatement)
            {
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    func.TypeDescriptor,
                    result.Expression,
                    result.Template
                ));
            }

            p.NonInlinePartWriter.WriteLine(result.Expression);

            var varName = BashVariableDefinitionStatementTranspiler.WriteLastStatusCodeStoreVariableDefinition(
                p.Context, p.Scope,
                p.NonInlinePartWriter, $"{functionCallStatement.Fqn}_Result");

            return new ApiMethodBuilderRawResult(new ExpressionResult(
                func.TypeDescriptor,
                $"${varName}",
                new VariableAccessStatement(varName, functionCallStatement.Info),
                ExpressionBuilderBase.PinRequiredNotice
            ));
        }


        public static IApiMethodBuilderResult CreateTestExpression(ApiBaseFunction func, ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement, TestExpressionCreator createTestExpression)
        {
            func.AssertParameters(p, functionCallStatement.Parameters);

            var result = createTestExpression(p, functionCallStatement);

            if (p.UsageContext is IfElseStatement)
            {
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    func.TypeDescriptor,
                    result.Expression,
                    result.Template
                ));
            }

            p.NonInlinePartWriter.WriteLine(result.Expression);

            var varName = BashVariableDefinitionStatementTranspiler.WriteLastStatusCodeStoreVariableDefinition(
                p.Context, p.Scope,
                p.NonInlinePartWriter, $"{functionCallStatement.Fqn}_Result");

            return new ApiMethodBuilderRawResult(new ExpressionResult(
                func.TypeDescriptor,
                $"${varName}",
                new VariableAccessStatement(varName, functionCallStatement.Info),
                ExpressionBuilderBase.PinRequiredNotice
            ));
        }
    }
}