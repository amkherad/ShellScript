namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionCallStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        
        public string FunctionName { get; }
        public EvaluationStatement[] Parameters { get; }
        
        
        public FunctionCallStatement(string functionName, EvaluationStatement[] parameters)
        {
            FunctionName = functionName;
            Parameters = parameters;
        }
    }
}