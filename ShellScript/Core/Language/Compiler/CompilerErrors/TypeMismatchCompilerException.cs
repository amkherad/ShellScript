using System;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.CompilerErrors
{
    public class TypeMismatchCompilerException : CompilerException
    {
        public TypeMismatchCompilerException(TypeDescriptor specifiedTypeDescriptor, TypeDescriptor expectedTypeDescriptor, StatementInfo info)
            : base(CreateMessage(specifiedTypeDescriptor, expectedTypeDescriptor, info), info)
        {
        }

        public TypeMismatchCompilerException(TypeDescriptor specifiedTypeDescriptor, TypeDescriptor expectedTypeDescriptor, StatementInfo info,
            Exception innerException)
            : base(CreateMessage(specifiedTypeDescriptor, expectedTypeDescriptor, info), info, innerException)
        {
        }

        public TypeMismatchCompilerException(PositionInfo positionInfo, Exception innerException)
            : base(positionInfo, innerException)
        {
        }

        public static string CreateMessage(TypeDescriptor specifiedTypeDescriptor, TypeDescriptor expectedTypeDescriptor, StatementInfo info)
        {
            // ReSharper disable once UseStringInterpolation
            return
                $"A type mismatch exception has been thrown, expectation was: '{expectedTypeDescriptor}' but a type of '{specifiedTypeDescriptor}' is specified {info}";
        }

//        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType)
//        {
//            return
//                $"A type mismatch exception has been thrown, expectation was: '{expectedType}' but a type of '{specifiedType}' is specified.";
//        }
    }
}