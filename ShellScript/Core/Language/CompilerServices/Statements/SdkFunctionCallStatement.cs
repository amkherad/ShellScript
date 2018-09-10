using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class SdkFunctionCallStatement : FunctionCallStatement
    {
        public const string RootClassName = "Root";

        public SdkFunctionCallStatement(string sdkClassName, string sdkFunctionName, DataTypes dataType,
            EvaluationStatement[] parameters, StatementInfo info)
            : base(sdkClassName, sdkFunctionName, dataType, parameters, info)
        {
        }

        public SdkFunctionCallStatement(string sdkRootFunctionName, DataTypes dataType,
            EvaluationStatement[] parameters, StatementInfo info)
            : base(RootClassName, sdkRootFunctionName, dataType, parameters, info)
        {
        }
    }
}