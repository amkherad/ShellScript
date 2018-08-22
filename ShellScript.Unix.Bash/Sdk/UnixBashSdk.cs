using ShellScript.Core.Language.Sdk;
using ShellScript.Unix.Bash.Sdk.ClassLibrary.IO;

namespace ShellScript.Unix.Bash.Sdk
{
    public class UnixBashSdk : SdkBase
    {
        public override ISdkVariable[] Variables => new ISdkVariable[0];
        public override ISdkFunc[] Functions => new ISdkFunc[0];
        public override ISdkClass[] Classes { get; } =
        {
            new SdkFile()
        };
        
        public override string Name => "Unix-Bash";
        public override string OutputFileExtension => "sh";
    }
}