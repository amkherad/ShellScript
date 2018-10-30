namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class NegativeNumberOperator : ArithmeticOperator
    {
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.RightToLeft;
        public override int Order => 1000;
        
        
        public NegativeNumberOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "-";
    }
}