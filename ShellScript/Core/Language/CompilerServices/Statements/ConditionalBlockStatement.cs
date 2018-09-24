namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConditionalBlockStatement : IStatement, IBlockWrapperStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }

        public EvaluationStatement Condition { get; }
        public IStatement Statement { get; }
        
        public IStatement[] TraversableChildren { get; protected set; }

        
        public ConditionalBlockStatement(EvaluationStatement condition, IStatement statement, StatementInfo info)
        {
            Condition = condition;
            Statement = statement;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(condition, statement);
        }
        
        public override string ToString()
        {
            return Condition?.ToString();
        }
    }
}