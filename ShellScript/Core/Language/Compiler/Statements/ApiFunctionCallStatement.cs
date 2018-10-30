using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class ApiFunctionCallStatement : FunctionCallStatement
    {
        public const string RootClassName = "Root";

        public ApiFunctionCallStatement(string sdkClassName, string sdkFunctionName, TypeDescriptor typeDescriptor,
            EvaluationStatement[] parameters, StatementInfo info)
            : base(sdkClassName, sdkFunctionName, typeDescriptor, parameters, info)
        {
        }

        public ApiFunctionCallStatement(string sdkRootFunctionName, TypeDescriptor typeDescriptor,
            EvaluationStatement[] parameters, StatementInfo info)
            : base(RootClassName, sdkRootFunctionName, typeDescriptor, parameters, info)
        {
        }
    }
}