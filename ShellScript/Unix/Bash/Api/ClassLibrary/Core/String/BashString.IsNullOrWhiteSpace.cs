using System.Globalization;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class BashString
    {
        public class BashIsNullOrWhiteSpace : IsNullOrWhiteSpace
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                var value = functionCallStatement.Parameters[0];
                var constant = value as ConstantValueStatement;
                if (constant != null)
                {
                    if (!constant.TypeDescriptor.IsString())
                        throw new TypeMismatchCompilerException(constant.TypeDescriptor, TypeDescriptor.String,
                            constant.Info);
                    return Inline(new ConstantValueStatement(TypeDescriptor.Boolean,
                        string.IsNullOrWhiteSpace(BashTranspilerHelpers.GetString(constant.Value))
                            .ToString(NumberFormatInfo.InvariantInfo), constant.Info));
                }

                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, (parameters, call) =>
                {
                    var transpiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[0]);
                    var result = transpiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, call.Parameters[0]);
                    return new ExpressionResult(TypeDescriptor.Boolean,
                        $"[[ {result.Expression} != *[![:space:]]* ]]", call);
                });
            }
        }
    }
}
