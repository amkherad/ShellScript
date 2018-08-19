using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class BlockStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public ParserInfo ParserInfo { get; }

        public IStatement[] Statements { get; }
        
        
        public BlockStatement(IStatement[] statements, ParserInfo parserInfo)
        {
            Statements = statements;
            ParserInfo = parserInfo;
        }
    }
}