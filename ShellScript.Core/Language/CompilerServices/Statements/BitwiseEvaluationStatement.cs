using System.Collections.Generic;
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


        public override IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                if (Left != null) yield return Left;
                yield return Operator;
                if (Right != null) yield return Right;
            }
        }



        public static BitwiseEvaluationStatement CreateNot(BitwiseNotOperator op, EvaluationStatement operand)
        {
            return new BitwiseEvaluationStatement(null, op, operand);
        }
    }
}