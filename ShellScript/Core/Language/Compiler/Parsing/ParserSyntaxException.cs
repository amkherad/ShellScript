using System;

namespace ShellScript.Core.Language.Compiler.Parsing
{
    public class ParserSyntaxException : ParserException
    {
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public ParserContext ParserContext { get; }

        public ParserSyntaxException(string message,
            int lineNumber, int columnNumber, ParserContext context)
            : base($"{message} '{context}' at {lineNumber}:{columnNumber}")
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserContext = context;
        }
        
        public ParserSyntaxException(string message,
            int lineNumber, int columnNumber, ParserContext context,
            Exception innerException)
            : base($"{message} '{context}' at {lineNumber}:{columnNumber}", innerException)
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserContext = context;
        }
        
        public ParserSyntaxException(int lineNumber, int columnNumber, ParserContext context)
            : base($"Parse exception '{context}' at {lineNumber}:{columnNumber}")
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserContext = context;
        }
    }
}