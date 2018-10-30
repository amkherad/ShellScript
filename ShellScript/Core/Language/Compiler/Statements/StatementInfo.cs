namespace ShellScript.Core.Language.Compiler.Statements
{
    public class StatementInfo : PositionInfo
    {
        public StatementInfo(string filePath, int lineNumber, int columnNumber)
            : base(filePath, lineNumber, columnNumber)
        {
        }
    }
}