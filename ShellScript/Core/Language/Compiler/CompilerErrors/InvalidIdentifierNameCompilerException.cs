using System;
using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.CompilerErrors
{
    public class InvalidIdentifierNameCompilerException : CompilerException
    {
        public InvalidIdentifierNameCompilerException(string identifierName, StatementInfo info)
            : base(CreateMessage(identifierName, info), info)
        {
        }

        public InvalidIdentifierNameCompilerException(string identifierName, StatementInfo info, Exception innerException)
            : base(CreateMessage(identifierName, info), info, innerException)
        {
        }

//        public InvalidIdentifierNameCompilerException(string message, Exception innerException) : base(message, innerException)
//        {
//        }
//
//        public InvalidIdentifierNameCompilerException(Exception innerException) : base(innerException)
//        {
//        }

        public static string CreateMessage(string identifierName, StatementInfo info)
        {
            return $"Invalid identifier name '{identifierName}' is provided {info}";
        }
//        public static string CreateMessage(string identifierName)
//        {
//            return $"Invalid identifier name '{identifierName}' is provided.";
//        }
    }
}