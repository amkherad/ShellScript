namespace ShellScript.Core.Language.CompilerServices.Statements.Operators
{
    public class ReminderOperator : ArithmeticOperator
    {
        public override int Order => 55;
        public override StatementInfo Info { get; }
        public override OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        
        
        public ReminderOperator(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString() => @"\";
    }
}