namespace ShellScript.Core.Language.Compiler.Statements
{
    public class WhileStatement : ConditionalBlockStatement, IBlockWrapperStatement
    {
        public WhileStatement(EvaluationStatement condition, IStatement statement, StatementInfo info)
            : base(condition, statement, info)
        {
        }

        public override string ToString()
        {
            return $"while ({Condition}) {{}}";
        }
    }
}