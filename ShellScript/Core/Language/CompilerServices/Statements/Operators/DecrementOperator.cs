namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class DecrementOperator : ArithmeticOperator
    {
        public override int Order => 65;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.ContextChangeable;
        
        
        public DecrementOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "--";
    }
}