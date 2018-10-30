namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ThrowStatement : IStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }
        public IStatement[] TraversableChildren => new IStatement[0];


        public ThrowStatement(StatementInfo info)
        {
            Info = info;
        }

        public override string ToString()
        {
            return $"throw ";
        }
    }
}