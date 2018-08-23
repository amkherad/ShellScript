using System;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class CompilerException : Exception
    {
        public CompilerException(string message)
            : base(message)
        {
        }

        public CompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public CompilerException(Exception innerException)
            : base("", innerException)
        {
        }
    }
}