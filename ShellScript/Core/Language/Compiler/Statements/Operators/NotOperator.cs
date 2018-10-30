namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class NotOperator : LogicalOperator
    {
        public override int Order => 65;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.RightToLeft;
        
        
        public NotOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "!";
    }
}