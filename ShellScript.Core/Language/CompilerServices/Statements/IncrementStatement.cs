namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IncrementStatement : EvaluationStatement
    {
        public override bool IsBlockStatement { get; }
        
        public VariableAccessStatement Variable { get; }
        
        
        public IncrementStatement(VariableAccessStatement variable)
        {
            Variable = variable;
        }
    }
}