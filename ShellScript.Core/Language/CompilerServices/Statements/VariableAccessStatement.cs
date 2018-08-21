namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableAccessStatement : IStatement
    {
        public bool IsBlockStatement => false;
        
        public string VariableName { get; }
        
        
        public VariableAccessStatement(string variableName)
        {
            VariableName = variableName;
        }
    }
}