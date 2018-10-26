using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public struct ExpressionResult
    {
        public DataTypes DataType { get; }
        public string Expression { get; }
        public EvaluationStatement Template { get; }

        public IExpressionNotice[] Notices { get; }


        public ExpressionResult(DataTypes dataType, string expression, EvaluationStatement template)
        {
            DataType = dataType;
            Expression = expression;
            Template = template;
            Notices = null;
        }

        public ExpressionResult(DataTypes dataType, string expression, EvaluationStatement template,
            IExpressionNotice[] notices)
        {
            DataType = dataType;
            Expression = expression;
            Template = template;
            Notices = notices;
        }

        public override string ToString()
        {
            return Expression;
        }

        public bool IsEmptyResult => DataType == DataTypes.Void && Expression == null && Template == null && Notices == null;
        public static ExpressionResult EmptyResult => new ExpressionResult(DataTypes.Void, null, null);
    }
}