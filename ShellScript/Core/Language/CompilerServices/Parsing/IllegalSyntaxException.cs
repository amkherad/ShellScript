namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class IllegalSyntaxException : ParserSyntaxException
    {
        public IllegalSyntaxException(string message, int lineNumber, int columnNumber, ParserInfo info)
            : base(message, lineNumber, columnNumber, info)
        {
        }
        public IllegalSyntaxException(int lineNumber, int columnNumber, ParserInfo info)
            : base(lineNumber, columnNumber, info)
        {
        }
    }
}