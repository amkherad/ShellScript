using ShellScript.Core.Language.CompilerServices.Statements.Operators;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class BitwiseEvaluationStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public EvaluationStatement Left { get; }
        public BitwiseOperator Operator { get; }
        public EvaluationStatement Right { get; }


        public BitwiseEvaluationStatement(EvaluationStatement left, BitwiseOperator @operator,
            EvaluationStatement right, StatementInfo info)
        {
            Left = left;
            Operator = @operator;
            Right = right;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(left, @operator, right);
        }

        public override string ToString()
        {
            return $"{Left} {Operator} {Right}";
        }

        public static BitwiseEvaluationStatement CreateNot(BitwiseNotOperator op, EvaluationStatement operand,
            StatementInfo info)
        {
            return new BitwiseEvaluationStatement(null, op, operand, info);
        }
    }
}