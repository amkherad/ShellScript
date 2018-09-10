namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class EvaluationStatement : IStatement
    {
        public abstract bool IsBlockStatement { get; }
        public abstract StatementInfo Info { get; }

        public IStatement[] TraversableChildren { get; protected set; }
    }
}