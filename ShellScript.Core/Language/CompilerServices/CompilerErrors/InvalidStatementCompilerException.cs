using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class InvalidStatementCompilerException : CompilerException
    {
        public InvalidStatementCompilerException(IStatement statement, StatementInfo info)
            : base(CreateMessage(statement, info), info)
        {
        }
        
        public static string CreateMessage(IStatement statement, StatementInfo info)
        {
            return
                $"Invalid statement of type '{statement?.GetType().Name}' was found in '{info?.FilePath}' at {info?.LineNumber}:{info?.ColumnNumber}.";
        }
    }
}