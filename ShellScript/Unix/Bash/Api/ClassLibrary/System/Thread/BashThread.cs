using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.System.Thread;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.System.Thread
{
    public partial class BashThread : ApiThread
    {
        public override IApiFunc[] Functions { get; } = {new BashGetCurrentId()};

        public class BashGetCurrentId : GetCurrentId
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                new ApiMethodBuilderRawResult(new ExpressionResult(TypeDescriptor, "$$", call));
        }
    }
}
