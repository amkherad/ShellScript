namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class BitwiseNotOperator : BitwiseOperator
    {
        public override int Order => 65;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.RightToLeft;
        
        
        public BitwiseNotOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "~";
    }
}