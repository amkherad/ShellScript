using System;

namespace ShellScript.Core.Language.CompilerServices
{
    public class InvalidIdentifierNameCompilerException : CompilerException
    {
        public InvalidIdentifierNameCompilerException(string message) : base(message)
        {
        }

        public InvalidIdentifierNameCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidIdentifierNameCompilerException(Exception innerException) : base(innerException)
        {
        }

        public static string CreateMessage(string identifierName)
        {
            return $"Invalid identifier name '{identifierName}' is provided.";
        }
    }
}