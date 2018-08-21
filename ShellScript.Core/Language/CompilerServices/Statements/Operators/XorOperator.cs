namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    /// <summary>
    /// Even if Xor is a binary operator it has logical effects on boolean types.
    /// </summary>
    public class XorOperator : BitwiseOperator
    {
        public override int Order => 38;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}