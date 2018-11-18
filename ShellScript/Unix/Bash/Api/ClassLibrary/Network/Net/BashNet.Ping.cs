using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net
{
    public partial class BashNet
    {
        public class BashPing : Ping
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var endpoint = functionCallStatement.Parameters[0];

                p.FormatString = false;

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(endpoint);
                var result = transpiler.GetExpression(p, endpoint);

                if (!result.TypeDescriptor.IsString())
                {
                    throw ThrowInvalidParameterType(result);
                }

                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor,
                    $"`ping {result.Expression}`",
                    result.Template
                ));
            }
        }
    }
}