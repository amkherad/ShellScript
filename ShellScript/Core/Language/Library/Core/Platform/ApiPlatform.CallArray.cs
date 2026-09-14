using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.Core.Platform
{
    public partial class ApiPlatform
    {
        public abstract class CallArray : Call
        {
            public override string Name => nameof(CallArray);
            public override string Summary =>
                "Executes a shell command and returns stdout split into an array of lines.";

            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
        }
    }
}
