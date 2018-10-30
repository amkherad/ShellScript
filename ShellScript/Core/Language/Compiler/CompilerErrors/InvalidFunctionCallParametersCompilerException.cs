using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.CompilerErrors
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