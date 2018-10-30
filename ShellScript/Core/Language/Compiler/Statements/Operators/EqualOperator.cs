namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class EqualOperator : LogicalOperator
    {
        public override int Order => 44;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public EqualOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "==";
    }
}