namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForEachStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }

        public IStatement Variable { get; }
        
        public VariableAccessStatement Iterator { get; }
        
        public IStatement Statement { get; }
        
        public IStatement[] TraversableChildren { get; }

        
        public ForEachStatement(IStatement variable, VariableAccessStatement iterator, IStatement statement, StatementInfo info)
        {
            Variable = variable;
            Iterator = iterator;
            Statement = statement;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(variable, iterator, statement);
        }
    }
}