namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SdkFunctionCallStatement : FunctionCallStatement
    {
        public const string RootClassName = "Root";

        public SdkFunctionCallStatement(string sdkClassName, string sdkFunctionName, EvaluationStatement[] parameters)
            : base(sdkClassName, sdkFunctionName, parameters)
        {
        }

        public SdkFunctionCallStatement(string sdkRootFunctionName, EvaluationStatement[] parameters)
            : base(RootClassName, sdkRootFunctionName, parameters)
        {
        }
    }
}