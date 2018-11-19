namespace ShellScript.Core.Language.Compiler.Statements.Operators
{
    public class IndexerOperator : IOperator
    {
        public bool CanBeEmbedded => false;
        public StatementInfo Info { get; }
        public OperatorAssociativity Associativity => OperatorAssociativity.LeftToRight;
        public int Order => 100;
        
        public IStatement[] TraversableChildren => StatementHelpers.EmptyStatements;
        
        public EvaluationStatement Indexer { get; }
        
        public IndexerOperator(EvaluationStatement indexer, StatementInfo info)
        {
            Info = info;
            Indexer = indexer;
        }

        public override string ToString() => "[]";
    }
}