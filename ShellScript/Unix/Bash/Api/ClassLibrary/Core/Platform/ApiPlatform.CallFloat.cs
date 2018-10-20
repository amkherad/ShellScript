using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallFloat : Call
        {
            public override string Name => nameof(CallFloat);
            public override string Summary { get; }
            public override DataTypes DataType => DataTypes.Float;
        }
    }
}