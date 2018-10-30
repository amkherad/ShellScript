using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net
{
    public partial class ApiNet
    {
        public class Ping : BashFunction
        {
            public override string Name => nameof(Ping);

            public override string Summary =>
                "Takes a string and execute it as a void-result platform-dependent shell command.";

            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;

            public override bool IsStatic => true;
            public override bool AllowDynamicParams => false;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Endpoint", null, null),
            };

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