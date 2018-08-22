using ShellScript.Core.Language.Sdk;

namespace ShellScript.Unix.Bash.Sdk.ClassLibrary.IO
{
    public partial class SdkFile : SdkBaseClass
    {
        public override string Name => "File";

        public override ISdkVariable[] Variables => new ISdkVariable[0];

        public override ISdkFunc[] Functions { get; } =
        {
            new Exists(),
        };
    }
}