namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class SubtractionOperator : ArithmeticOperator
    {
        public override int Order => 50;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public SubtractionOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "-";
    }
}