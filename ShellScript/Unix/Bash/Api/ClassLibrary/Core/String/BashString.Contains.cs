using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class BashString
    {
        public class BashContains : Contains
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, (parameters, call) =>
                {
                    var leftTranspiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[0]);
                    var rightTranspiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[1]);
                    var left = leftTranspiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, call.Parameters[0]);
                    var right = rightTranspiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, call.Parameters[1]);
                    return new ExpressionResult(TypeDescriptor,
                        $"[[ {0} == *{1}* ]]".Replace("{0}", left.Expression).Replace("{1}", right.Expression), call);
                });
            }
        }
    }
}
