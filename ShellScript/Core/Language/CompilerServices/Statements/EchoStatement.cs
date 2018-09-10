using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class EchoStatement : SdkFunctionCallStatement
    {
        public EchoStatement(
            EvaluationStatement[] parameters, StatementInfo info) : base(RootClassName, "echo", DataTypes.Void,
            parameters, info)
        {
        }
    }
}