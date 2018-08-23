using System.Collections.Generic;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ArithmeticEvaluationStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        public override StatementInfo Info { get; }

        public EvaluationStatement Left { get; }
        public ArithmeticOperator Operator { get; }
        public EvaluationStatement Right { get; }



        public ArithmeticEvaluationStatement(EvaluationStatement left, ArithmeticOperator @operator, EvaluationStatement right, StatementInfo info)
        {
            Left = left;
            Operator = @operator;
            Right = right;
            Info = info;
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
        

        public static ArithmeticEvaluationStatement CreatePostfixIncrement(IncrementOperator op, EvaluationStatement operand, StatementInfo info)
        {
            return new ArithmeticEvaluationStatement(operand, op, null, info);
        }

        public static ArithmeticEvaluationStatement CreatePrefixIncrement(IncrementOperator op, EvaluationStatement operand, StatementInfo info)
        {
            return new ArithmeticEvaluationStatement(null, op, operand, info);
        }

        
        public static ArithmeticEvaluationStatement CreatePostfixDecrement(DecrementOperator op, EvaluationStatement operand, StatementInfo info)
        {
            return new ArithmeticEvaluationStatement(operand, op, null, info);
        }

        public static ArithmeticEvaluationStatement CreatePrefixDecrement(DecrementOperator op, EvaluationStatement operand, StatementInfo info)
        {
            return new ArithmeticEvaluationStatement(null, op, operand, info);
        }
    }
}
