using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO
{
    public partial class ApiFile
    {
        public class Exists : ApiBaseFunction
        {
            public override string Name => "Exists";

            public override bool IsStatic => true;
            public override bool AllowDynamicParams => false;

            public override IApiParameter[] Parameters { get; } =
            {
                new ApiParameter("FilePath", DataTypes.String,
                    "A valid path to the file to check whether it exists or not."),
            };


            public override IApiMethodBuilderResult BuildGeneral(Context context, Scope scope, TextWriter metaWriter,
                TextWriter nonInlinePartWriter, EvaluationStatement[] parameters)
            {
                throw new System.NotImplementedException();
            }

            public override IApiMethodBuilderResult BuildIfCondition(Context context, Scope scope,
                TextWriter metaWriter, TextWriter nonInlinePartWriter, EvaluationStatement[] parameters)
            {
                AssertParameters(parameters);

                var expBuilder = new BashEvaluationStatementTranspiler.StringConcatenationExpressionBuilder();

                var exp = expBuilder.CreateExpression(context, scope, nonInlinePartWriter, parameters[0]);

                return new ApiMethodBuilderRawResult($"-f {exp}");
            }
        }
    }
}