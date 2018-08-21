namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class AdditionOperator : ArithmeticOperator
    {
        public override int Order => 50;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}