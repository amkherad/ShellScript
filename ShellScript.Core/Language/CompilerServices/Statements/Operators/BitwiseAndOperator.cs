namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class BitwiseAndOperator : BitwiseOperator
    {
        public override int Order => 39;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}