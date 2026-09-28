using System.Collections.Generic;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;
using ShellScript.Windows.PowerShell.Api.ClassLibrary.Diagnostics;

namespace ShellScript.Windows.PowerShell.Api
{
    public class WindowsPowerShellApi : ApiBase
    {
        public override string Name => "Windows-PowerShell";

        public override IApiVariable[] Variables => new IApiVariable[0];
        public override IApiFunc[] Functions => new IApiFunc[0];

        public override IApiClass[] Classes { get; } =
        {
            new PowerShellConsole(),
        };

        public override IDictionary<string, IThirdPartyUtility> Utilities { get; } =
            new Dictionary<string, IThirdPartyUtility>();
    }
}
