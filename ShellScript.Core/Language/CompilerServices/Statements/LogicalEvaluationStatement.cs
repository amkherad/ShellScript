using ShellScript.Core.Language.CompilerServices.Statements.Operators;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class LogicalEvaluationStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;

        public EvaluationStatement Left { get; }
        public LogicalOperator Operator { get; }
        public EvaluationStatement Right { get; }
        
        
        public LogicalEvaluationStatement(EvaluationStatement left, LogicalOperator @operator, EvaluationStatement right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
        
        
        public static LogicalEvaluationStatement CreateNot(NotOperator op, EvaluationStatement operand)
        {
            return new LogicalEvaluationStatement(null, op, operand);
        }
    }
}