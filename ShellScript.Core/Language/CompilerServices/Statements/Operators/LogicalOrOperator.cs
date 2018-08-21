namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class LogicalOrOperator : LogicalOperator
    {
        public override int Order => 35;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}