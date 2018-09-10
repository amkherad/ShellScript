namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class StatementInfo : PositionInfo
    {
        public StatementInfo(string filePath, int lineNumber, int columnNumber)
            : base(filePath, lineNumber, columnNumber)
        {
        }
    }
}