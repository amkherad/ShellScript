using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallDecimal : Call
        {
            public override string Name => nameof(CallDecimal);
            public override string Summary { get; }
            public override DataTypes DataType => DataTypes.Decimal;
        }
    }
}