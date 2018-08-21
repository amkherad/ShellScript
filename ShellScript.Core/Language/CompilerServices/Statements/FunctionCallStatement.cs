namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionCallStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        
        public string ClassName { get; }
        public string FunctionName { get; }
        public EvaluationStatement[] Parameters { get; }
        
        
        public FunctionCallStatement(string className, string functionName, EvaluationStatement[] parameters)
        {
            ClassName = className;
            FunctionName = functionName;
            Parameters = parameters;
        }
    }
}