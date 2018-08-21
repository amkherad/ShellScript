namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SdkFunctionCallStatement : FunctionCallStatement
    {
        public SdkFunctionCallStatement(string sdkClassName, string sdkFunctionName, EvaluationStatement[] parameters)
            : base(sdkClassName, sdkFunctionName, parameters)
        {
        }
    }
}