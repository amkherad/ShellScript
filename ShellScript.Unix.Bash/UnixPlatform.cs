using ShellScript.Core.Language;
using ShellScript.Core.Language.Sdk;
using ShellScript.Unix.Bash.Sdk;

namespace ShellScript.Unix.Bash
{
    public class UnixPlatform : IPlatform
    {
        static UnixPlatform()
        {
            Platforms.AddPlatform(new UnixPlatform());
        }

        public string Name => "Unix";

        public ISdk[] Sdks { get; } = {new UnixBashSdk()};
    }
}