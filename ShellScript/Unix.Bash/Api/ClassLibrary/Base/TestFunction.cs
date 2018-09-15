using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public abstract class TestFunction : ApiBaseFunction
    {
        protected abstract string CreateTestExpression(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement);

        public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            var exp = CreateTestExpression(p, functionCallStatement);

            return new ApiMethodBuilderRawResult(DataType, exp);
        }
    }
}