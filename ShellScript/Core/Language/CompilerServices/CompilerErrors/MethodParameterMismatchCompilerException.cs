using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class MethodParameterMismatchCompilerException : CompilerException
    {
        public MethodParameterMismatchCompilerException(string functionName, StatementInfo info)
            : base(CreateMessage(functionName, info), info)
        {
        }

        private static string CreateMessage(string functionName, StatementInfo info)
        {
            return $"Invalid parameters passed to '{functionName}' {info}";
        }
    }
}