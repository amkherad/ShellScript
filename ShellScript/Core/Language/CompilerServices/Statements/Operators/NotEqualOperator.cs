namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class NotEqualOperator : LogicalOperator
    {
        public override int Order => 44;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public NotEqualOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "!=";
    }
}