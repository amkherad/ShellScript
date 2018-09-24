using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ApiFunctionCallStatement : FunctionCallStatement
    {
        public const string RootClassName = "Root";

        public ApiFunctionCallStatement(string sdkClassName, string sdkFunctionName, DataTypes dataType,
            EvaluationStatement[] parameters, StatementInfo info)
            : base(sdkClassName, sdkFunctionName, dataType, parameters, info)
        {
        }

        public ApiFunctionCallStatement(string sdkRootFunctionName, DataTypes dataType,
            EvaluationStatement[] parameters, StatementInfo info)
            : base(RootClassName, sdkRootFunctionName, dataType, parameters, info)
        {
        }
    }
}