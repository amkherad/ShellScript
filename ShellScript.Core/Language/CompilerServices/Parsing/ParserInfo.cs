namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class ParserInfo
    {
        public bool SemicolonRequired { get; }
        
        public string FileName { get; }
        public string FilePath { get; }
        public string File { get; }
        
        public ParserInfo(bool semicolonRequired, string file, string fileName, string filePath)
        {
            SemicolonRequired = semicolonRequired;
            File = file;
            FileName = fileName;
            FilePath = filePath;
        }
    }
}