using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class LogicalAndStatement : LogicalOperatorStatement
    {
        public LogicalStatement Left { get; }
        public LogicalStatement Right { get; }
        
        public LogicalAndStatement(LogicalStatement left, LogicalStatement right, ParserInfo info)
            : base(info)
        {
            Left = left;
            Right = right;
        }
    }
}