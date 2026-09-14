using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Path
{
    public partial class BashPath
    {
        public class BashGetTempPath : GetTempPath
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor, "${TMPDIR:-/tmp}", functionCallStatement));
            }
        }

        public class BashGetFullPath : GetFullPath
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return BashApiNative.Native(this, p, functionCallStatement, ClassAccessName,
                    "readlink -f -- \"$1\" 2>/dev/null || realpath -- \"$1\"");
            }
        }

        public class BashIsPathRooted : IsPathRooted
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, (parameters, call) =>
                {
                    var transpiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[0]);
                    var result = transpiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, call.Parameters[0]);
                    return new ExpressionResult(TypeDescriptor, $"[[ {result.Expression} == /* ]]", call);
                });
            }
        }
    }
}
