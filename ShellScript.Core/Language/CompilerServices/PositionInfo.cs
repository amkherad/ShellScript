namespace ShellScript.Core.Language.CompilerServices
{
    public class PositionInfo
    {
        public string FilePath { get; }
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        
        
        public PositionInfo(string filePath, int lineNumber, int columnNumber)
        {
            FilePath = filePath;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        public override string ToString()
        {
            return $"in {FilePath} at {LineNumber}:{ColumnNumber}";
        }
    }
}