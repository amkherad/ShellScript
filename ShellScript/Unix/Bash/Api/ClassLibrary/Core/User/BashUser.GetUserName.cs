using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.User
{
    public partial class BashUser
    {
        public class BashGetUserName : GetUserName
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, (p1, fcs) =>
                    new ExpressionResult(
                        TypeDescriptor,
                        "`whoami`",
                        functionCallStatement
                    ));
            }
        }
    }
}