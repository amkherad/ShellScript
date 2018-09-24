using System;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class IdentifierNotFoundCompilerException : CompilerException
    {
        public IdentifierNotFoundCompilerException(string identifierName, StatementInfo info)
            : base(CreateMessage(identifierName, info), info)
        {
        }
        
        public IdentifierNotFoundCompilerException(VariableAccessStatement variableAccessStatement)
            : base(CreateMessage(variableAccessStatement.VariableName, variableAccessStatement.Info), variableAccessStatement.Info)
        {
        }
        
        public IdentifierNotFoundCompilerException(FunctionCallStatement functionCallStatement)
            : base(CreateMessage(functionCallStatement.Fqn, functionCallStatement.Info), functionCallStatement.Info)
        {
        }
        
        public IdentifierNotFoundCompilerException(string identifierName, StatementInfo info, Exception innerException)
            : base(CreateMessage(identifierName, info), info, innerException)
        {
        }

//        public IdentifierNotFoundCompilerException(string message, Exception innerException) : base(message, innerException)
//        {
//        }
//
//        public IdentifierNotFoundCompilerException(Exception innerException) : base(innerException)
//        {
//        }

        public static string CreateMessage(string identifierName, StatementInfo info)
        {
            return $"Identifier '{identifierName}' does not found {info}";
        }
//        public static string CreateMessage(string identifierName)
//        {
//            return $"Identifier '{identifierName}' does not found.";
//        }
    }
}