namespace ShellScript.Core.Language.Compiler.Statements
{
    public class IncludeStatement : IStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }
        public IStatement[] TraversableChildren { get; }
        
        public EvaluationStatement Target { get; }
        
        public IncludeStatement(EvaluationStatement target, StatementInfo info)
        {
            Target = target;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(target);
        }
    }
}