using System;

namespace ShellScript.Core.Language.Compiler
{
    public class CompilationResult
    {
        public bool Successful { get; }
        
        public Exception Exception { get; set; }
        
        public CompilationResult(bool successful)
        {
            Successful = successful;
        }
    }
}