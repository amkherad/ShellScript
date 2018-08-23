namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SdkFunctionCallStatement : FunctionCallStatement
    {
        public const string RootClassName = "Root";

        public SdkFunctionCallStatement(string sdkClassName, string sdkFunctionName, EvaluationStatement[] parameters, StatementInfo info)
            : base(sdkClassName, sdkFunctionName, parameters, info)
        {
        }

        public SdkFunctionCallStatement(string sdkRootFunctionName, EvaluationStatement[] parameters, StatementInfo info)
            : base(RootClassName, sdkRootFunctionName, parameters, info)
        {
        }
    }
}