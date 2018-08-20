using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableAccessStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public ParserInfo ParserInfo { get; }
        
        public string VariableName { get; }
        
        
        public VariableAccessStatement(string variableName, ParserInfo parserInfo)
        {
            VariableName = variableName;
            ParserInfo = parserInfo;
        }
    }
}