using System;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class InvalidOperatorForTypeCompilerException : CompilerException
    {
        public InvalidOperatorForTypeCompilerException(Type operatorType, DataTypes dataType)
            : base(CreateMessage(operatorType, dataType))
        {
        }
        
        public InvalidOperatorForTypeCompilerException(string message)
            : base(message)
        {
        }

        public InvalidOperatorForTypeCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidOperatorForTypeCompilerException(Exception innerException) : base(innerException)
        {
        }

        public static string CreateMessage(Type operatorType, DataTypes dataType)
        {
            return $"Invalid operator '{operatorType.Name}' on variable of type '{dataType}'.";
        }
    }
}