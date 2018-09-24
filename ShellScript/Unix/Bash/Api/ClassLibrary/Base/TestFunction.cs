using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public abstract class TestFunction : BashFunction
    {
        protected abstract string CreateTestExpression(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement);

        public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            AssertParameters(functionCallStatement.Parameters);
            
            var exp = CreateTestExpression(p, functionCallStatement);

            return new ApiMethodBuilderRawResult(DataType, exp);
        }
    }
}