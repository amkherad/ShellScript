using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.Library
{
    public class ApiMethodBuilderRawResult : IApiMethodBuilderResult
    {
        public DataTypes DataType { get; }
        public string Expression { get; }
        public EvaluationStatement Template { get; }

        public ApiMethodBuilderRawResult(DataTypes dataType, string expression, EvaluationStatement template)
        {
            DataType = dataType;
            Expression = expression;
            Template = template;
        }
    }
}