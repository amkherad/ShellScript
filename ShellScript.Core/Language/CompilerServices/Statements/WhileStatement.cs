namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class WhileStatement : ConditionalBlockStatement
    {
        public WhileStatement(IStatement condition, IStatement statement)
            : base(condition, statement)
        {
        }
    }
}