namespace ShellScript.Core.Language.Compiler.Parsing
{
    public class IllegalSyntaxException : ParserSyntaxException
    {
        public IllegalSyntaxException(string message, int lineNumber, int columnNumber, ParserContext context)
            : base(message, lineNumber, columnNumber, context)
        {
        }
        public IllegalSyntaxException(int lineNumber, int columnNumber, ParserContext context)
            : base(lineNumber, columnNumber, context)
        {
        }
    }
}