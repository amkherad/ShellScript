namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class IllegalSyntaxException : ParserSyntaxException
    {
        public IllegalSyntaxException(int lineNumber, int columnNumber, ParserInfo info)
            : base(lineNumber, columnNumber, info)
        {
        }
    }
}