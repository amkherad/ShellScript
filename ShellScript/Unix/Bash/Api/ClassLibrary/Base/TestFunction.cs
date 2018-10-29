using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public abstract class TestFunction : BashFunction
    {
        protected abstract ExpressionResult CreateTestExpression(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement);

        public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            AssertParameters(p, functionCallStatement.Parameters);

            var result = CreateTestExpression(p, functionCallStatement);

            if (p.UsageContext is IfElseStatement)
            {
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor,
                    result.Expression,
                    result.Template
                ));
            }
            
            p.NonInlinePartWriter.WriteLine(result.Expression);

            var varName = BashVariableDefinitionStatementTranspiler.WriteLastStatusCodeStoreVariableDefinition(p.Context, p.Scope,
                p.NonInlinePartWriter, $"{functionCallStatement.Fqn}_Result");
            
            return new ApiMethodBuilderRawResult(new ExpressionResult(
                TypeDescriptor,
                $"${varName}",
                new VariableAccessStatement(varName, functionCallStatement.Info),
                ExpressionBuilderBase.PinRequiredNotice
            ));
        }
    }
}