namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class LogicalAndOperator : LogicalOperator
    {
        public override int Order => 36;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}