namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class BitwiseOrOperator : BitwiseOperator
    {
        public override int Order => 37;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public BitwiseOrOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "|";
    }
}