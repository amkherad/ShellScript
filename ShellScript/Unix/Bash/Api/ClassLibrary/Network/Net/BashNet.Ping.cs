using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net
{
    public partial class BashNet
    {
        public class BashPing : Ping
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, (parameters, call) =>
                {
                    var endpoint = call.Parameters[0];
                    var transpiler = parameters.Context.GetEvaluationTranspilerForStatement(endpoint);
                    var result = transpiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, endpoint);
                    return new ExpressionResult(TypeDescriptor,
                        $"ping -c 1 -W 1 -- {result.Expression} >/dev/null 2>&1", call);
                });
            }
        }
    }
}
