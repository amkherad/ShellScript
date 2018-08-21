using ShellScript.Core.Language.CompilerServices.Statements.Operators;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ArithmeticEvaluationStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        
        public EvaluationStatement Left { get; }
        public ArithmeticOperator Operator { get; }
        public EvaluationStatement Right { get; }
        
        
        public ArithmeticEvaluationStatement(EvaluationStatement left, ArithmeticOperator @operator, EvaluationStatement right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
    }
}