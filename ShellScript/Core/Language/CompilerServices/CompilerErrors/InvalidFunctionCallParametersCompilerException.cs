using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class InvalidFunctionCallParametersCompilerException : CompilerException
    {
        public InvalidFunctionCallParametersCompilerException(int expectedCount, int passedCount, StatementInfo info)
            : base(CreateMessage(expectedCount, passedCount, info), info)
        {
        }
        
        public static string CreateMessage(int expectedCount, int passedCount, StatementInfo info)
        {
            return
                $"Invalid number of parameters passed to function {info}";
        }
    }
}