namespace ShellScript.Core.Language.Compiler.Statements
{
    public class IndexerAccessStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }
        
        public EvaluationStatement Source { get; }
        public EvaluationStatement Indexer { get; }
        
        
        public IndexerAccessStatement(EvaluationStatement source, EvaluationStatement indexer, StatementInfo info)
        {
            Source = source;
            Indexer = indexer;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(source, indexer);
        }

        public override string ToString() => $"{Source}[{Indexer}]";
    }
}