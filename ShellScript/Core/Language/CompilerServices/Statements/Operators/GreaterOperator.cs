namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class GreaterOperator : LogicalOperator
    {
        public override int Order => 45;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;


        public GreaterOperator(StatementInfo info)
        {
            Info = info;
        }
    }
}