using System.IO;

namespace ShellScript.Core.Language.CompilerServices.Parsing
{
    public class ParserContext
    {
        public TextWriter WarningWriter { get; }
        public TextWriter LogWriter { get; }
        
        public bool SemicolonRequired { get; }
        
        public string FileName { get; }
        public string FilePath { get; }
        public string File { get; }
        
        public ParserContext(
            TextWriter warningWriter,
            TextWriter logWriter,
            bool semicolonRequired,
            string file,
            string fileName,
            string filePath)
        {
            WarningWriter = warningWriter;
            LogWriter = logWriter;
            
            SemicolonRequired = semicolonRequired;
            File = file;
            FileName = fileName;
            FilePath = filePath;
        }

        public override string ToString()
        {
            return $"\"in {File}\"";
        }
    }
}