namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class ParserInfo
    {
        public bool SemicolonRequired { get; }
        
        public string FileName { get; }
        public string FilePath { get; }
        
        public ParserInfo(bool semicolonRequired, string fileName, string filePath)
        {
            SemicolonRequired = semicolonRequired;
            FileName = fileName;
            FilePath = filePath;
        }
    }
}