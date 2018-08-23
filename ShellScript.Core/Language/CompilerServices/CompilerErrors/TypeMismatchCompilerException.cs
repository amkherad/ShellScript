using System;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
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

        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType, StatementInfo info)
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format(
                "A type mismatch exception has been thrown, expectation was: '{0}' but a type of '{1}' is specified in '{2}' at {3}:{4}.",
                expectedType,
                specifiedType,
                info.FilePath,
                info.LineNumber,
                info.ColumnNumber
            );
        }

        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType)
        {
            return
                $"A type mismatch exception has been thrown, expectation was: '{expectedType}' but a type of '{specifiedType}' is specified.";
        }
    }
}