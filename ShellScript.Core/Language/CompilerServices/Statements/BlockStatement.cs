using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
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