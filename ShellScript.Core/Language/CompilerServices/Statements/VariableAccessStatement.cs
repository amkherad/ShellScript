namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableAccessStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        
        public string VariableName { get; }
        
        
        public VariableAccessStatement(string variableName)
        {
            VariableName = variableName;
        }
    }
}