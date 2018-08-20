using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForEachStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public ParserInfo ParserInfo { get; }
        
        public IStatement Variable { get; }
        
        public VariableAccessStatement Iterator { get; }
        
        
        public ForEachStatement(IStatement variable, VariableAccessStatement iterator, ParserInfo parserInfo)
        {
            Variable = variable;
            Iterator = iterator;
            ParserInfo = parserInfo;
        }
    }
}