namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class DoWhileStatement : ConditionalBlockStatement
    {
        public DoWhileStatement(IStatement condition, IStatement statement)
            : base(condition, statement)
        {
        }
    }
}