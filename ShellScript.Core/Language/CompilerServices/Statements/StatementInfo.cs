namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class StatementInfo
    {
        public string FilePath { get; }
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        
        
        public StatementInfo(string filePath, int lineNumber, int columnNumber)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }
    }
}