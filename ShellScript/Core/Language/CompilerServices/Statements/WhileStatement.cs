namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class WhileStatement : ConditionalBlockStatement, IBlockWrapperStatement
    {
        public WhileStatement(EvaluationStatement condition, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
        }
    }
}