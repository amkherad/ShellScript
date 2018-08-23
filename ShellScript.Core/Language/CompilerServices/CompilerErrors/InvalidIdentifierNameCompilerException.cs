using System;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class InvalidIdentifierNameCompilerException : CompilerException
    {
        public InvalidIdentifierNameCompilerException(string identifierName, StatementInfo info)
            : base(CreateMessage(identifierName, info))
        {
        }

        public InvalidIdentifierNameCompilerException(string identifierName, StatementInfo info, Exception innerException)
            : base(CreateMessage(identifierName, info), innerException)
        {
        }

        public InvalidIdentifierNameCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidIdentifierNameCompilerException(Exception innerException) : base(innerException)
        {
        }

        public static string CreateMessage(string identifierName, StatementInfo info)
        {
            return $"Invalid identifier name '{identifierName}' is provided in '{info.FilePath}' at {info.LineNumber}:{info.ColumnNumber}";
        }
        public static string CreateMessage(string identifierName)
        {
            return $"Invalid identifier name '{identifierName}' is provided.";
        }
    }
}