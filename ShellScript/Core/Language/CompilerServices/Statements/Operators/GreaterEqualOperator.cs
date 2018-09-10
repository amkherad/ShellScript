namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class GreaterEqualOperator : LogicalOperator
    {
        public override int Order => 45;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;


        public GreaterEqualOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => ">=";
    }
}