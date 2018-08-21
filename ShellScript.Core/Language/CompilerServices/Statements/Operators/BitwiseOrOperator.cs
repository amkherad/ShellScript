namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class BitwiseOrOperator : BitwiseOperator
    {
        public override int Order => 37;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}