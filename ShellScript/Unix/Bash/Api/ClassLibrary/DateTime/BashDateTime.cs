using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.DateTime;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.DateTime
{
    public partial class BashDateTime : ApiDateTime
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashNow(),
            new BashUtcNow(),
            new BashFormat(),
            new BashToUnixTime(),
        };

        public class BashNow : Now
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "date '+%Y-%m-%d %H:%M:%S'");
        }

        public class BashUtcNow : UtcNow
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "date -u '+%Y-%m-%d %H:%M:%S'");
        }

        public class BashFormat : Format
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "date -d \"@$1\" +\"$2\"");
        }

        public class BashToUnixTime : ToUnixTime
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "date +%s");
        }
    }
}
