using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallArray : Call
        {
            public override string Name => nameof(CallArray);
            public override string Summary { get; }
            public override DataTypes DataType => DataTypes.Decimal;


            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertExpressionParameters(functionCallStatement.Parameters);

                var parameter = functionCallStatement.Parameters[0];

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
                var result = transpiler.GetExpression(p.Context, p.Scope, p.MetaWriter, p.NonInlinePartWriter,
                    p.UsageContext, parameter);

                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    DataType,
                    $"`awk \"BEGIN {result.Expression}\"`",
                    result.Template
                ));
            }
        }
    }
}