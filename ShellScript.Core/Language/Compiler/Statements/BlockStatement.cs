namespace ShellScript.Core.Language.Compiler.Statements
{
    public class BlockStatement : IStatement
    {
        public bool IsBlockStatement => true;
        
        public IStatement[] Statements { get; }
        
        
        public BlockStatement(IStatement[] statements)
        {
            Statements = statements;
        }
    }
}