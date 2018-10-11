using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class Call : BashFunction
        {
            public override string Name => nameof(Call);

            public override string Summary =>
                "Takes a string and execute it as a void-result platform-dependent shell command.";

            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Void;

            public override bool IsStatic => true;
            public override bool AllowDynamicParams => false;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(DataTypes.String, "RawCommand", null, null),
            };

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(functionCallStatement.Parameters);

                var parameter = functionCallStatement.Parameters[0];

                p.FormatString = false;

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
                var result = transpiler.GetExpression(p, parameter);

                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    DataType,
                    $"`{result.Expression}`",
                    result.Template
                ));
            }
        }
    }
}