using System.Diagnostics;

namespace ShellScript.Core.Language.Compiler.Lexing
{
    [DebuggerDisplay("{Type}: ({Value})")]
    public class Token
    {
        public string Value { get; }
        
        public TokenType Type { get; }
        
        public int ColumnOffset { get; }
        public int LineNumber { get; }
        
        public Token(string value, TokenType type, int columnOffset, int lineNumber)
        {
            Value = value;
            Type = type;
            ColumnOffset = columnOffset;
            LineNumber = lineNumber;
        }
    }
}