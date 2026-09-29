using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Network.Net;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net
{
    public partial class BashNet : ApiNet
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashPing(),
            new BashDownload(),
            new BashHttpGet(),
            new BashResolveHost(),
            new BashGetOpenSockets(),
            new BashGetSocketProcessId(),
            new BashGetNetworkInterfaces(),
            new BashGetOpenSocketsOnInterface(),
        };
    }
}