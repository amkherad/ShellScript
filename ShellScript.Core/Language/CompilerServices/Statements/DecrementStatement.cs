namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class DecrementStatement : EvaluationStatement
    {
        public override bool IsBlockStatement { get; }
        
        public VariableAccessStatement Variable { get; }
        
        
        public DecrementStatement(VariableAccessStatement variable)
        {
            Variable = variable;
        }
    }
}