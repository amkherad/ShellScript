using System;
using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.Parsing
{
    public class ParserSyntaxException : ParserException
    {
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public ParserContext ParserContext { get; }

        public ParserSyntaxException(string message,
            int lineNumber, int columnNumber, ParserContext context)
            : base($"{message} {context} at {lineNumber}:{columnNumber}")
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ParserContext = context;
        }
        
        public ParserSyntaxException(string message,
            int lineNumber, int columnNumber, ParserContext context,
            Exception innerException)
            : base($"{message} {context} at {lineNumber}:{columnNumber}", innerException)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ParserContext = context;
        }
        
        public ParserSyntaxException(int lineNumber, int columnNumber, ParserContext context)
            : base($"Parse exception {context} at {lineNumber}:{columnNumber}")
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ParserContext = context;
        }

        public ParserSyntaxException(string message, StatementInfo info, ParserContext context)
            : base(message)
        {
            LineNumber = info?.LineNumber ?? 0;
            ColumnNumber = info?.ColumnNumber ?? 0;
            ParserContext = context;
        }
    }
}