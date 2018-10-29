using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallNumeric : Call
        {
            public override string Name => nameof(CallNumeric);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
        }
    }
}