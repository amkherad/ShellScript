using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.System.Process;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.System.Process
{
    public partial class BashProcess : ApiProcess
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashGetCurrentId(),
            new BashGetParentId(),
            new BashExists(),
            new BashKill(),
            new BashRun(),
            new BashRunAndCapture(),
            new BashGetName(),
        };

        public class BashGetCurrentId : GetCurrentId
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor, "$$", call));
        }

        public class BashGetParentId : GetParentId
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "ps -o ppid= -p $$ | tr -d ' '");
        }

        public class BashExists : Exists
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashTestCommand.CreateTestExpression(this, p, call, (parameters, statement) =>
                    new ExpressionResult(TypeDescriptor, "kill -0 \"$1\" 2>/dev/null", statement));
        }

        public class BashKill : Kill
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "kill \"$1\"");
        }

        public class BashRun : Run
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "eval \"$1\"");
        }

        public class BashRunAndCapture : RunAndCapture
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "eval \"$1\"");
        }

        public class BashGetName : GetName
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "ps -p \"$1\" -o comm= 2>/dev/null | tr -d ' '");
        }
    }
}
