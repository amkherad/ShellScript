namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class EqualOperator : LogicalOperator
    {
        public override int Order => 44;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}