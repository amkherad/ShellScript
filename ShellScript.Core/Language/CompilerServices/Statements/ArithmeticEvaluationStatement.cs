using System.Collections.Generic;
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


        public override IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                if (Left != null) yield return Left;
                yield return Operator;
                if (Right != null) yield return Right;
            }
        }
        

        public static ArithmeticEvaluationStatement CreatePostfixIncrement(IncrementOperator op, EvaluationStatement operand)
        {
            return new ArithmeticEvaluationStatement(operand, op, null);
        }

        public static ArithmeticEvaluationStatement CreatePrefixIncrement(IncrementOperator op, EvaluationStatement operand)
        {
            return new ArithmeticEvaluationStatement(null, op, operand);
        }

        
        public static ArithmeticEvaluationStatement CreatePostfixDecrement(DecrementOperator op, EvaluationStatement operand)
        {
            return new ArithmeticEvaluationStatement(operand, op, null);
        }

        public static ArithmeticEvaluationStatement CreatePrefixDecrement(DecrementOperator op, EvaluationStatement operand)
        {
            return new ArithmeticEvaluationStatement(null, op, operand);
        }
    }
}
