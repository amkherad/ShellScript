namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class ParserSyntaxException : ParserException
    {
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public ParserInfo ParserInfo { get; }

        public ParserSyntaxException(int lineNumber, int columnNumber, ParserInfo info)
            : base($"Parse exception in '{info?.File}' at {lineNumber}:{columnNumber}")
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserInfo = info;
        }
    }
}