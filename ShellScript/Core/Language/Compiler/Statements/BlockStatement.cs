namespace ShellScript.Core.Language.Compiler.Statements
{
    public class BlockStatement : IStatement
    {
        public bool CanBeEmbedded => true;
        public StatementInfo Info { get; }

        public IStatement[] Statements { get; }

        public IStatement[] TraversableChildren { get; protected set; }
        
        
        public BlockStatement(IStatement[] statements, StatementInfo info)
        {
            Statements = statements;
            Info = info;

            TraversableChildren = statements;
        }
    }
}