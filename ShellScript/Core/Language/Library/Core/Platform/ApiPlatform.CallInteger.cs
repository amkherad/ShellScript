namespace ShellScript.Core.Language.Library.Core.Platform
{
    public partial class ApiPlatform
    {
        public abstract class CallInteger : Call
        {
            public override string Name => nameof(CallInteger);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
        }
    }
}