using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;

namespace ShellScript.Core.Language.Library
{
    public class ApiMethodBuilderRawResult : IApiMethodBuilderResult
    {
        public ExpressionResult Result { get; }

        public ApiMethodBuilderRawResult(ExpressionResult result)
        {
            Result = result;
        }
    }
}