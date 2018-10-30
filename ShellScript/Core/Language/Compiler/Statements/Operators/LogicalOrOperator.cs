namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class LogicalOrOperator : LogicalOperator
    {
        public override int Order => 35;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public LogicalOrOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "||";
    }
}