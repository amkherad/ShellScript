using ShellScript.Core.Language.CompilerServices.Statements.Operators;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class LogicalEvaluationStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public EvaluationStatement Left { get; }
        public LogicalOperator Operator { get; }
        public EvaluationStatement Right { get; }
        
        
        public LogicalEvaluationStatement(EvaluationStatement left, LogicalOperator @operator, EvaluationStatement right, StatementInfo info)
        {
            Left = left;
            Operator = @operator;
            Right = right;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(left, @operator, right);
        }
       
        
        public static LogicalEvaluationStatement CreateNot(NotOperator op, EvaluationStatement operand, StatementInfo info)
        {
            return new LogicalEvaluationStatement(null, op, operand, info);
        }
    }
}