using System;
using ShellScript.Core.Language.Compiler.Parsing;

namespace ShellScript.Core.Language.Compiler.PreProcessors
{
    public class PreProcessorException : ParserException
    {
        public PreProcessorException(string message) : base(message)
        {
        }

        public PreProcessorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}