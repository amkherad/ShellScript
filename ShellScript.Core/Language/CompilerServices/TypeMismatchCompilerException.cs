using System;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices
{
    public class TypeMismatchCompilerException : CompilerException
    {
        public TypeMismatchCompilerException(DataTypes specifiedType, DataTypes expectedType)
            : base(CreateMessage(specifiedType, expectedType))
        {
        }

        public TypeMismatchCompilerException(DataTypes specifiedType, DataTypes expectedType, Exception innerException)
            : base(CreateMessage(specifiedType, expectedType), innerException)
        {
        }

        public TypeMismatchCompilerException(Exception innerException) : base(innerException)
        {
        }

        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType)
        {
            return
                $"A type mismatch exception has been thrown, expectation was: '{expectedType}' but a type of '{specifiedType}' is specified.";
        }
    }
}