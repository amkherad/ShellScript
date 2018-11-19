namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class DecrementOperator : ArithmeticOperator
    {
        public override int Order => 80;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.Contextual;
        
        
        public DecrementOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "--";
    }
}