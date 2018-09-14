namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class DoWhileStatement : ConditionalBlockStatement, IBlockWrapperStatement
    {
        public DoWhileStatement(EvaluationStatement condition, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
        }
    }
}