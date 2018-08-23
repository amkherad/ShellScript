namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class DivisionOperator : ArithmeticOperator
    {
        public override int Order => 55;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;


        public DivisionOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "/";
    }
}