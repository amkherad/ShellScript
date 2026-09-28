using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Diagnostics;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Diagnostics
{
    public partial class BashLog : ApiLog
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashInfo(),
            new BashWarn(),
            new BashError(),
            new BashDebug(),
        };

        public class BashInfo : Info
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '[INFO] %s\\n' \"$1\" >&2");
        }

        public class BashWarn : Warn
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '[WARN] %s\\n' \"$1\" >&2");
        }

        public class BashError : Error
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '[ERROR] %s\\n' \"$1\" >&2");
        }

        public class BashDebug : Debug
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '[DEBUG] %s\\n' \"$1\" >&2");
        }
    }
}
