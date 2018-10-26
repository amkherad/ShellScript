using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public struct PinnedVariableResult
    {
        public DataTypes DataType { get; }
        public string Name { get; }
        public string Expression { get; }
        public EvaluationStatement Template { get; }

        public PinnedVariableResult(DataTypes dataType, string name, string expression, EvaluationStatement template)
        {
            DataType = dataType;
            Name = name;
            Expression = expression;
            Template = template;
        }
        
        public static implicit operator ExpressionResult(PinnedVariableResult result)
        {
            return new ExpressionResult(
                result.DataType,
                result.Expression,
                result.Template
                );
        }
    }
}