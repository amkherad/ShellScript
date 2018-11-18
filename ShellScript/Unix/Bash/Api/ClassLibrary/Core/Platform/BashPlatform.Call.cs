using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class BashPlatform
    {
        public class BashCall : Call
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) => BuildHelper(this, p, functionCallStatement);
            
            public static IApiMethodBuilderResult BuildHelper(Call call, ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                call.AssertParameters(p, functionCallStatement.Parameters);

                var parameter = functionCallStatement.Parameters[0];

                p.FormatString = false;

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
                var result = transpiler.GetExpression(p, parameter);

                if (!result.TypeDescriptor.IsString())
                {
                    throw ThrowInvalidParameterType(result);
                }

                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    call.TypeDescriptor,
                    $"`{result.Expression}`",
                    result.Template
                ));
            }
        }
    }
}