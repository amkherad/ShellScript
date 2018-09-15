namespace ShellScript.Core.Language.Library
{
    public class ApiMethodBuilderRawResult : IApiMethodBuilderResult
    {
        public DataTypes DataType { get; }
        public string Expression { get; }

        public ApiMethodBuilderRawResult(DataTypes dataType, string expression)
        {
            DataType = dataType;
            Expression = expression;
        }
    }
}