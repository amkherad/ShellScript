namespace ShellScript.Core.Language.Compiler.Parsing
{
    public class IllegalSyntaxException : ParserSyntaxException
    {
        public IllegalSyntaxException(int lineNumber, int columnNumber, ParserInfo info)
            : base(lineNumber, columnNumber, info)
        {
        }
    }
}