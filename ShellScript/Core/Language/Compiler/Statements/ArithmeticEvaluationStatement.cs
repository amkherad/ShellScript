using ShellScript.Core.Language.Compiler.Statements.Operators;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ArithmeticEvaluationStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded { get; }
        public override StatementInfo Info { get; }

        public EvaluationStatement Left { get; }
        public ArithmeticOperator Operator { get; }
        public EvaluationStatement Right { get; }


        public ArithmeticEvaluationStatement(EvaluationStatement left, ArithmeticOperator @operator,
            EvaluationStatement right, StatementInfo info, IStatement parentStatement = null)
        {
            CanBeEmbedded = false;
            Left = left;
            Operator = @operator;
            Right = right;
            Info = info;
            ParentStatement = parentStatement;

            TraversableChildren = StatementHelpers.CreateChildren(left, @operator, right);
        }
        private ArithmeticEvaluationStatement(
            bool canBeEmbedded,
            EvaluationStatement left, ArithmeticOperator @operator,
            EvaluationStatement right, StatementInfo info, IStatement parentStatement = null)
        {
            CanBeEmbedded = canBeEmbedded;
            Left = left;
            Operator = @operator;
            Right = right;
            Info = info;
            ParentStatement = parentStatement;

            TraversableChildren = StatementHelpers.CreateChildren(left, @operator, right);
        }

        public override string ToString()
        {
            return $"{Left} {Operator} {Right}";
        }

        public static ArithmeticEvaluationStatement CreateNegate(NegativeNumberOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(null, op, operand, info)
            {
                ParentStatement = parentStatement
            };
        }
        
        public static ArithmeticEvaluationStatement CreatePostfixIncrement(IncrementOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(true, operand, op, null, info)
            {
                ParentStatement = parentStatement
            };
        }

        public static ArithmeticEvaluationStatement CreatePrefixIncrement(IncrementOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(true, null, op, operand, info)
            {
                ParentStatement = parentStatement
            };
        }


        public static ArithmeticEvaluationStatement CreatePostfixDecrement(DecrementOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(true, operand, op, null, info)
            {
                ParentStatement = parentStatement
            };
        }

        public static ArithmeticEvaluationStatement CreatePrefixDecrement(DecrementOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(true, null, op, operand, info)
            {
                ParentStatement = parentStatement
            };
        }


        public static ArithmeticEvaluationStatement CreatePostfix(ArithmeticOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(operand, op, null, info)
            {
                ParentStatement = parentStatement
            };
        }

        public static ArithmeticEvaluationStatement CreatePrefix(ArithmeticOperator op,
            EvaluationStatement operand, StatementInfo info, IStatement parentStatement = null)
        {
            return new ArithmeticEvaluationStatement(null, op, operand, info)
            {
                ParentStatement = parentStatement
            };
        }
    }
}