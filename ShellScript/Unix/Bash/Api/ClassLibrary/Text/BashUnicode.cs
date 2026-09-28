using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Text;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Text
{
    public partial class BashUnicode : ApiUnicode
    {
        public override IApiFunc[] Functions { get; } = {new BashGetLength()};

        public class BashGetLength : GetLength
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "python3 -c 'import sys; print(len(sys.argv[1]))' \"$1\"");
        }
    }
}
