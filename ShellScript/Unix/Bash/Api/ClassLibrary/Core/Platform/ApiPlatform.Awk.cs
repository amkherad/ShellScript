using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class Awk : BashFunction
        {
            private const string ApiMathAbsBashMethodName = "Awk_Bash";

            public override string Name => "Awk";
            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Numeric;

            public override bool IsStatic => true;
            public override bool AllowDynamicParams => false;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                null,
            };

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertExpressionParameters(functionCallStatement.Parameters);

                var parameter = functionCallStatement.Parameters[0];

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
                var (dataType, exp, template) = transpiler.GetInline(p.Context, p.Scope, p.MetaWriter,
                    p.NonInlinePartWriter, p.UsageContext, parameter);

                return new ApiMethodBuilderRawResult(dataType,
                    $"`awk \"BEGIN {exp}\"`", template);
            }
        }
    }
}