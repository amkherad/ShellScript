namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ReturnStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public StatementInfo Info { get; }

        public EvaluationStatement Statement { get; }
        
        public IStatement[] TraversableChildren { get; protected set; }

        
        public ReturnStatement(EvaluationStatement statement, StatementInfo info)
        {
            Statement = statement;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(statement);
        }
    }
}