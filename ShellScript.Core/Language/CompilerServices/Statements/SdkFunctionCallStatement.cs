namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SdkFunctionCallStatement : FunctionCallStatement
    {
        public SdkFunctionCallStatement(string sdkFunctionName, EvaluationStatement[] parameters)
            : base(sdkFunctionName, parameters)
        {
        }
    }
}