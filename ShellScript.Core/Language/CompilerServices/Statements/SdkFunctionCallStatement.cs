namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SdkFunctionCallStatement : FunctionCallStatement
    {
        public SdkFunctionCallStatement(string sdkFunctionName, IStatement[] parameters)
            : base(sdkFunctionName, parameters)
        {
        }
    }
}