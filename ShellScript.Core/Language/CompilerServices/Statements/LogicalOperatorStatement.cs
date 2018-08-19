using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class LogicalOperatorStatement : LogicalStatement
    {
        public override ParserInfo ParserInfo { get; }
        
        public LogicalOperatorStatement(ParserInfo parserInfo)
        {
            ParserInfo = parserInfo;
        }
    }
}