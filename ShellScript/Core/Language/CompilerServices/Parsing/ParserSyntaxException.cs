using System;

namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class ParserSyntaxException : ParserException
    {
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public ParserInfo ParserInfo { get; }

        public ParserSyntaxException(string message,
            int lineNumber, int columnNumber, ParserInfo info)
            : base($"{message} in '{info?.File}' at {lineNumber}:{columnNumber}")
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserInfo = info;
        }
        
        public ParserSyntaxException(string message,
            int lineNumber, int columnNumber, ParserInfo info,
            Exception innerException)
            : base($"{message} in '{info?.File}' at {lineNumber}:{columnNumber}", innerException)
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserInfo = info;
        }
        
        public ParserSyntaxException(int lineNumber, int columnNumber, ParserInfo info)
            : base($"Parse exception in '{info?.File}' at {lineNumber}:{columnNumber}")
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserInfo = info;
        }
    }
}