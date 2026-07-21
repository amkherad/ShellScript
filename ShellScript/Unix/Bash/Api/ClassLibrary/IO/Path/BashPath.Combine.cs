using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Path
{
    public partial class BashPath
    {
        public class BashCombine : Combine
        {
            private readonly FunctionInfo _functionInfo = new FunctionInfo(TypeDescriptor.String,
                "Combine", null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "if [[ -z $1 ]]; then\n    printf '%s' \"$2\"\nelif [[ -z $2 ]]; then\n    printf '%s' \"$1\"\nelif [[ $1 == */ ]]; then\n    printf '%s%s' \"$1\" \"${2#/}\"\nelse\n    printf '%s/%s' \"$1\" \"${2#/}\"\nfi", _functionInfo,
                    functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
