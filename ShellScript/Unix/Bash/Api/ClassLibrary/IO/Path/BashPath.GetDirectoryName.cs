using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Path
{
    public partial class BashPath
    {
        public class BashGetDirectoryName : GetDirectoryName
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String,
                "GetDirectoryName", null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "if [[ $1 == / ]]; then\n    printf '/'\nelse\n    path=${1%/}\n    if [[ $path == */* ]]; then\n        directory=${path%/*}\n        printf '%s' \"${directory:-/}\"\n    else\n        printf '.'\n    fi\nfi", FunctionInfo,
                    functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
