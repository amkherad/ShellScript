namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class IncrementOperator : ArithmeticOperator
    {
        public override int Order => 80;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.Contextual;
        
        
        public IncrementOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "++";
    }
}