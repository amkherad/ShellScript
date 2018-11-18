using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert
{
    public partial class BashConvert
    {
        public class BashToFloat : ToFloat
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var param = functionCallStatement.Parameters[0];

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(param);
                var result = transpiler.GetExpression(p, param);
                
                return new ApiMethodBuilderRawResult(result);
            }
        }
    }
}