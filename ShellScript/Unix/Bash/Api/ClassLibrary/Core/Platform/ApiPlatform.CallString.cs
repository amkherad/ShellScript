using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallString : Call
        {
            public override string Name => nameof(CallString);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;

        }
    }
}