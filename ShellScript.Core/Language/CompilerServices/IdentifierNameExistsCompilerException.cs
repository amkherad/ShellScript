namespace ShellScript.Core.Language.CompilerServices
{
    public class IdentifierNameExistsCompilerException : CompilerException
    {
        public string VariableName { get; }
        
        
        public IdentifierNameExistsCompilerException(string variableName)
            : base(CreateMessage(variableName))
        {
            VariableName = variableName;
        }

        public static string CreateMessage(string variableName)
        {
            return $"Identifier name '{variableName}' already exists in current scope.";
        }
    }
}