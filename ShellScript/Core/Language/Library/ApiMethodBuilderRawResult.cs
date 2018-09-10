namespace ShellScript.Core.Language.Library
{
    public class ApiMethodBuilderRawResult : IApiMethodBuilderResult
    {
        public string Expression { get; }
        
        
        public ApiMethodBuilderRawResult(string expression)
        {
            Expression = expression;
        }
    }
}