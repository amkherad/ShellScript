namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class BitwiseNotOperator : BitwiseOperator
    {
        public override int Order => 65;
        public override OperatorAssociativity Associativity => OperatorAssociativity.RightToLeft;
    }
}