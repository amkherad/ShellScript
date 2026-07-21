using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Environment
{
    public partial class BashEnvironment
    {
        public class BashGetCurrentDirectory : GetCurrentDirectory
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor, "${PWD}", functionCallStatement));
            }
        }
    }
}
