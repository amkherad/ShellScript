using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Platform;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class BashPlatform : ApiPlatform
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashCall(),
            new BashCallInteger(),
            new BashCallFloat(),
            new BashCallNumeric(),
            new BashCallString(),
            
            //new CallArray(),
        };
    }
}