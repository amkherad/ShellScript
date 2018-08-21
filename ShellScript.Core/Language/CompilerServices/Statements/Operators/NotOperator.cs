namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class NotOperator : LogicalOperator
    {
        public override int Order => 60;
        public override OperatorAssociativity Associativity => OperatorAssociativity.RightToLeft;
    }
}