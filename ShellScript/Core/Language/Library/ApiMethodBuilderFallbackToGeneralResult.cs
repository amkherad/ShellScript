namespace ShellScript.Core.Language.Library
{
    public class ApiMethodBuilderFallbackToGeneralResult : IApiMethodBuilderResult
    {
        public static ApiMethodBuilderFallbackToGeneralResult Instance { get; } =
            new ApiMethodBuilderFallbackToGeneralResult();

        private ApiMethodBuilderFallbackToGeneralResult()
        {
            
        }
    }
}