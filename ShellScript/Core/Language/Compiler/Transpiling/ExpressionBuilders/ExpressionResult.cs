using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders
{
    public readonly struct ExpressionResult
    {
        public TypeDescriptor TypeDescriptor { get; }
        public string Expression { get; }
        public EvaluationStatement Template { get; }

        public IExpressionNotice[] Notices { get; }


        public ExpressionResult(TypeDescriptor typeDescriptor, string expression, EvaluationStatement template)
        {
            TypeDescriptor = typeDescriptor;
            Expression = expression;
            Template = template;
            Notices = null;
        }

        public ExpressionResult(TypeDescriptor typeDescriptor, string expression, EvaluationStatement template,
            IExpressionNotice[] notices)
        {
            TypeDescriptor = typeDescriptor;
            Expression = expression;
            Template = template;
            Notices = notices;
        }

        public override string ToString()
        {
            return Expression;
        }

        public bool IsEmptyResult => TypeDescriptor.IsVoid() && Expression == null && Template == null && Notices == null;
        public static ExpressionResult EmptyResult => new ExpressionResult(TypeDescriptor.Void, null, null);
    }
}