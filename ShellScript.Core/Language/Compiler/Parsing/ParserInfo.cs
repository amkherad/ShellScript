namespace ShellScript.Core.Language.Compiler.Parsing
{
    public class ParserInfo
    {
        public string FileName { get; }
        public string FilePath { get; }
        
        public ParserInfo(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }
}