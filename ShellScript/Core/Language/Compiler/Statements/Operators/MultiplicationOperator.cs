namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class MultiplicationOperator : ArithmeticOperator
    {
        public override int Order => 55;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public MultiplicationOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => "*";
    }
}