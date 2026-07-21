using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Path
{
    public partial class BashPath
    {
        public class BashGetExtension : GetExtension
        {
            private readonly FunctionInfo _functionInfo = new FunctionInfo(TypeDescriptor.String,
                "GetExtension", null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "path=${1%/}\nname=${path##*/}\nif [[ $name == *.* && $name != .* && $name != *. ]]; then\n    printf '.%s' \"${name##*.}\"\nfi", _functionInfo,
                    functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
