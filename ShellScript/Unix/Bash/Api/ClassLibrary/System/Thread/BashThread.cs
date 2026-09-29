using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.System.Thread;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.System.Thread
{
    public partial class BashThread : ApiThread
    {
        public override IApiFunc[] Functions { get; } = {new BashGetCurrentId(), new BashSleep()};

        public class BashGetCurrentId : GetCurrentId
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor, "$$", call));
        }

        public class BashSleep : Sleep
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "sleep $(awk -v ms=\"$1\" 'BEGIN{printf \"%.3f\", ms/1000}')");
        }
    }
}
