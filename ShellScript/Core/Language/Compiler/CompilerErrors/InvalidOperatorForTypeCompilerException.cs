using System;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.CompilerErrors
{
    public class InvalidOperatorForTypeCompilerException : CompilerException
    {
        public InvalidOperatorForTypeCompilerException(Type operatorType, TypeDescriptor typeDescriptor, StatementInfo statementInfo)
            : base(CreateMessage(operatorType, typeDescriptor), statementInfo)
        {
        }
        public InvalidOperatorForTypeCompilerException(Type operatorType, TypeDescriptor a, TypeDescriptor b, StatementInfo statementInfo)
            : base(CreateMessage(operatorType, a, b), statementInfo)
        {
        }

        public InvalidOperatorForTypeCompilerException(string message, StatementInfo statementInfo)
            : base(message, statementInfo)
        {
        }
        
//        public InvalidOperatorForTypeCompilerException(string message)
//            : base(message)
//        {
//        }
//
//        public InvalidOperatorForTypeCompilerException(string message, Exception innerException) : base(message, innerException)
//        {
//        }
//
//        public InvalidOperatorForTypeCompilerException(Exception innerException) : base(innerException)
//        {
//        }

        public static string CreateMessage(Type operatorType, TypeDescriptor a, TypeDescriptor b)
        {
            return $"Invalid operator '{operatorType.Name}' on variables of types of '{a}' and '{b}'.";
        }

        public static string CreateMessage(Type operatorType, TypeDescriptor typeDescriptor)
        {
            return $"Invalid operator '{operatorType.Name}' on variable of type of '{typeDescriptor}'.";
        }
        
        public static string CreateMessageForConstant(Type operatorType)
        {
            return $"Invalid operator '{operatorType.Name}' on constant value.";
        }
    }
}