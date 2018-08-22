using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices.Compiling;
using ShellScript.Core.Language.Sdk;
using ShellScript.Unix.Bash.PlatformTranspiler;
using ShellScript.Unix.Bash.Sdk;

namespace ShellScript.Unix.Bash
{
    public class UnixBashPlatform : IPlatform
    {
        public string Name => "Unix-Bash";

        public ISdk Sdk { get; } = new UnixBashSdk();

        public IPlatformStatementTranspiler[] Transpilers { get; } =
        {
            new IfElseTranspiler()
        };
    }
}