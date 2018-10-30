namespace ShellScript.Core.Language.Compiler.Statements
{
    public class DoWhileStatement : ConditionalBlockStatement, IBlockWrapperStatement
    {
        public DoWhileStatement(EvaluationStatement condition, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
        }

        public override string ToString()
        {
            return $"do {{ }} while ({Condition});";
        }
    }
}