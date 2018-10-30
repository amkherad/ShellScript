namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class BitwiseAndOperator : BitwiseOperator
    {
        public override int Order => 39;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public BitwiseAndOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "&";
    }
}