using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Environment;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Environment
{
    public partial class BashEnvironment : ApiEnvironment
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashGetVariable(),
            new BashGetCurrentDirectory(),
            new BashGetHomeDirectory(),
        };
    }
}
