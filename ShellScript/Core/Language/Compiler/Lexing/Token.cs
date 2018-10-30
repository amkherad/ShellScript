using System.Diagnostics;

namespace ShellScript.Core.Language.Compiler.Lexing
{
    [DebuggerDisplay("{Type}: ({Value})")]
    public class Token : PositionInfo
    {
        public string Value { get; }
        
        public TokenType Type { get; }
        
        public int ColumnStart { get; }
        public int ColumnEnd { get; }
        
        public Token(string value, TokenType type, int columnStart, int columnEnd, int lineNumber)
            : base(null, lineNumber, columnStart)
        {
            Value = value;
            Type = type;
            ColumnStart = columnStart;
            ColumnEnd = columnEnd;
        }
    }
}