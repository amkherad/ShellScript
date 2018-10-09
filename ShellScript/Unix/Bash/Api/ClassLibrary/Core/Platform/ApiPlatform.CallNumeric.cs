using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallNumeric : Call
        {
            public override string Name => nameof(CallNumeric);
            public override string Summary { get; }
            public override DataTypes DataType => DataTypes.Numeric;

        }
    }
}