using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.CompilerErrors
{
    public class IdentifierNameExistsCompilerException : CompilerException
    {
        public string VariableName { get; }
        
        
        public IdentifierNameExistsCompilerException(string variableName, StatementInfo info)
            : base(CreateMessage(variableName, info), info)
        {
            VariableName = variableName;
        }

        public static string CreateMessage(string variableName, StatementInfo info)
        {
            return $"Identifier name '{variableName}' already exists in current scope in '{info?.FilePath}' at {info?.LineNumber}:{info?.ColumnNumber}";
        }
//        public static string CreateMessage(string variableName)
//        {
//            return $"Identifier name '{variableName}' already exists in current scope.";
//        }
    }
}