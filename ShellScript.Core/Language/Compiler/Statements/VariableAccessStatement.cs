using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class VariableAccessStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public ParserInfo ParserInfo { get; }
        
        
        public VariableAccessStatement(ParserInfo parserInfo)
        {
            ParserInfo = parserInfo;
        }
    }
}