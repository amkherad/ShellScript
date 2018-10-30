namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class LessEqualOperator : LogicalOperator
    {
        public override int Order => 45;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;


        public LessEqualOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "<=";
    }
}