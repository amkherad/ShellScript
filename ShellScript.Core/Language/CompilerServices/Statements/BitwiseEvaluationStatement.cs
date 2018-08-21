using ShellScript.Core.Language.CompilerServices.Statements.Operators;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class BitwiseEvaluationStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;

        public EvaluationStatement Left { get; }
        public BitwiseOperator Operator { get; }
        public EvaluationStatement Right { get; }
        
        
        public BitwiseEvaluationStatement(EvaluationStatement left, BitwiseOperator @operator, EvaluationStatement right)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }
    }
}