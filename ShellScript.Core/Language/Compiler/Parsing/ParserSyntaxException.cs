namespace ShellScript.Core.Language.Compiler.Parsing
{
    public class ParserSyntaxException : ParserException
    {
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public ParserInfo ParserInfo { get; }

        public ParserSyntaxException(int lineNumber, int columnNumber, ParserInfo info)
        {
            LineNumber = LineNumber;
            ColumnNumber = columnNumber;
            ParserInfo = info;
        }
    }
}