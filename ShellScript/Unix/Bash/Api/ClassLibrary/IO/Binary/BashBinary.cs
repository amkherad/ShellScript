using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.IO.Binary;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Binary
{
    public partial class BashBinary : ApiBinary
    {
        public override IApiFunc[] Functions { get; } = {new BashToBase64(), new BashFromBase64()};

        public class BashToBase64 : ToBase64
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s' \"$1\" | base64 -w 0");
        }

        public class BashFromBase64 : FromBase64
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s' \"$1\" | base64 -d");
        }
    }
}
