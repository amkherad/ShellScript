namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class DecrementStatement : EvaluationStatement
    {
        public override bool IsBlockStatement { get; }
        
        public VariableAccessStatement Variable { get; }
        
        public bool IsPostfix { get; }
        
        
        public DecrementStatement(VariableAccessStatement variable, bool isPostfix)
        {
            Variable = variable;
            IsPostfix = isPostfix;
        }
    }
}