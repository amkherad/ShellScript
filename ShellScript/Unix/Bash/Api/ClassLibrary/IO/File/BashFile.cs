using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.IO.File;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class BashFile : ApiFile
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashExists(),
            new BashCanRead(),
            new BashCanWrite(),
            new BashCanExecute(),
            new BashIsLink(),
        };
    }
}