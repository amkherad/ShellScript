using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.IO.Path;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Path
{
    public partial class BashPath : ApiPath
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashCombine(),
            new BashGetFileName(),
            new BashGetDirectoryName(),
            new BashGetExtension(),
            new BashGetTempPath(),
            new BashIsPathRooted(),
            new BashGetFullPath(),
        };
    }
}
