namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class LessEqualOperator : LogicalOperator
    {
        public override int Order => 45;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}