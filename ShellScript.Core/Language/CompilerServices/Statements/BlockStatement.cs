namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class BlockStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }

        public IStatement[] Statements { get; }

        public IStatement[] TraversableChildren => Statements;
        
        
        public BlockStatement(IStatement[] statements, StatementInfo info)
        {
            Statements = statements;
            Info = info;
        }
    }
}