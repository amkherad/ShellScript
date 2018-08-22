using ShellScript.Core.Language.Sdk;

namespace ShellScript.Unix.Bash.Sdk.ClassLibrary.IO
{
    public partial class SdkFile
    {
        public class Exists : SdkBaseFunction
        {
            public override string Name => "Exists";

            public override bool IsStatic => true;
            public override bool AllowDynamicParams => false;

            public override ISdkParameter[] Parameters { get; } =
            {
                new SdkParameter("FilePath", DataTypes.String), 
            };
        }
    }
}