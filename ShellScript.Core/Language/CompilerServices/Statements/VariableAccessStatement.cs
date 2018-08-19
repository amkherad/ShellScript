using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
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