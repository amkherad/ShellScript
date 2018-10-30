namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ForEachStatement : IStatement, IBlockWrapperStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }

        public IStatement Variable { get; }
        
        public EvaluationStatement Iterator { get; }
        
        public IStatement Statement { get; }
        
        public IStatement[] TraversableChildren { get; }

        
        public ForEachStatement(IStatement variable, EvaluationStatement iterator, IStatement statement, StatementInfo info)
        {
            Variable = variable;
            Iterator = iterator;
            Statement = statement;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(variable, iterator, statement);
        }

        public override string ToString()
        {
            return $"foreach({Variable} in {Iterator})";
        }
    }
}