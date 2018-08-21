namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class ModulusOperator : ArithmeticOperator
    {
        public override int Order => 55;
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
    }
}