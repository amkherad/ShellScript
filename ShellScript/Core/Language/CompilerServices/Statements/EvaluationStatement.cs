namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class EvaluationStatement : IStatement
    {
        public abstract bool CanBeEmbedded { get; }
        public abstract StatementInfo Info { get; }

        public IStatement[] TraversableChildren { get; protected set; }

        public IStatement ParentStatement { get; set; }
    }
}