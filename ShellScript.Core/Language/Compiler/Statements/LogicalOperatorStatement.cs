using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.Statements
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