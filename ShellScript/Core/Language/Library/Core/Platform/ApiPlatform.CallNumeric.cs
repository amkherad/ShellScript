namespace ShellScript.Core.Language.Library.Core.Platform
{
    public partial class ApiPlatform
    {
        public abstract class CallNumeric : Call
        {
            public override string Name => nameof(CallNumeric);
            public override string Summary { get; }
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
        }
    }
}