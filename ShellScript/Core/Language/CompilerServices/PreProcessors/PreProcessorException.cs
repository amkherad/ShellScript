using System;
using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.Core.Language.CompilerServices.PreProcessors
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