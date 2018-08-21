namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionCallStatement : IStatement
    {
        public bool IsBlockStatement => false;
        
        public string SdkFunctionName { get; }
        public IStatement[] Parameters { get; }
        
        
        public FunctionCallStatement(string sdkFunctionName, IStatement[] parameters)
        {
            SdkFunctionName = sdkFunctionName;
            Parameters = parameters;
        }
    }
}