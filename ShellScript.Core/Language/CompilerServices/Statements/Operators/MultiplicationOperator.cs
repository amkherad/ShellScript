namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class MultiplicationOperator : ArithmeticOperator
    {
        public override int Order => 55;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}