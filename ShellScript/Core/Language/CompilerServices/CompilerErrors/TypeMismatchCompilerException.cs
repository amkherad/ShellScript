using System;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class TypeMismatchCompilerException : CompilerException
    {
        public TypeMismatchCompilerException(DataTypes specifiedType, DataTypes expectedType, StatementInfo info)
            : base(CreateMessage(specifiedType, expectedType, info), info)
        {
        }

        public TypeMismatchCompilerException(DataTypes specifiedType, DataTypes expectedType, StatementInfo info,
            Exception innerException)
            : base(CreateMessage(specifiedType, expectedType, info), info, innerException)
        {
        }

        public TypeMismatchCompilerException(PositionInfo positionInfo, Exception innerException)
            : base(positionInfo, innerException)
        {
        }

        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType, StatementInfo info)
        {
            // ReSharper disable once UseStringInterpolation
            return
                $"A type mismatch exception has been thrown, expectation was: '{expectedType}' but a type of '{specifiedType}' is specified {info}";
        }

//        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType)
//        {
//            return
//                $"A type mismatch exception has been thrown, expectation was: '{expectedType}' but a type of '{specifiedType}' is specified.";
//        }
    }
}