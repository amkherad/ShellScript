using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.IO.Directory;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Directory
{
    public partial class BashDirectory : ApiDirectory
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashExists(),
            new BashCreate(),
            new BashDelete(),
            new BashDeleteRecursive(),
            new BashGetFiles(),
            new BashGetDirectories(),
            new BashCopy(),
            new BashMove(),
        };
    }
}
