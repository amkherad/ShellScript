namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class WhileStatement : ConditionalBlockStatement
    {
        public WhileStatement(IStatement condition, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
        }
    }
}