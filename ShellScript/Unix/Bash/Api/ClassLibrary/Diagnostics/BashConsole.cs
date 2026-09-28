using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Diagnostics;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Diagnostics
{
    public partial class BashConsole : ApiConsole
    {
        public override IApiFunc[] Functions { get; } = {new BashWriteLine(), new BashWriteError()};

        public class BashWriteLine : WriteLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s\\n' \"$1\"");
        }

        public class BashWriteError : WriteError
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s\\n' \"$1\" >&2");
        }
    }
}
