namespace ShellScript.Core.Language.Library.Core.Platform
{
    public partial class ApiPlatform
    {
        public abstract class CallString : Call
        {
            public override string Name => nameof(CallString);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;

        }
    }
}