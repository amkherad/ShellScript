namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    /// <summary>
    /// Even if Xor is a binary operator it has logical effects on boolean types.
    /// </summary>
    public class XorOperator : BitwiseOperator
    {
        public override int Order => 38;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public XorOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "^";
    }
}