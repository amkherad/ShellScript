namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class LogicalAndOperator : LogicalOperator
    {
        public override int Order => 36;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public LogicalAndOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "&&";
    }
}