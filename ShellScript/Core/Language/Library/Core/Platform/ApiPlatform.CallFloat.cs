namespace ShellScript.Core.Language.Library.Core.Platform
{
    public partial class ApiPlatform
    {
        public abstract class CallFloat : Call
        {
            public override string Name => nameof(CallFloat);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Float;
        }
    }
}