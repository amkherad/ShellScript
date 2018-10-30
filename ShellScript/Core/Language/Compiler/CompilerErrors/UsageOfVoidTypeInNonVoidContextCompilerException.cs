using System;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.CompilerErrors
{
    public class UsageOfVoidTypeInNonVoidContextCompilerException : CompilerException
    {
        public UsageOfVoidTypeInNonVoidContextCompilerException(IStatement statement, PositionInfo positionInfo)
            : base(CreateMessage(statement.Info), positionInfo)
        {
        }
        
        public UsageOfVoidTypeInNonVoidContextCompilerException(PositionInfo positionInfo, Exception innerException)
            : base(positionInfo, innerException)
        {
        }

        public static string CreateMessage(StatementInfo info)
        {
            // ReSharper disable once UseStringInterpolation
            return
                $"A void type used when it shouldn't {info}";
        }

//        public static string CreateMessage(DataTypes specifiedType, DataTypes expectedType)
//        {
//            return
//                $"A type mismatch exception has been thrown, expectation was: '{expectedType}' but a type of '{specifiedType}' is specified.";
//        }
    }
}