using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallInteger : Call
        {
            public override string Name => nameof(CallInteger);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
        }
    }
}