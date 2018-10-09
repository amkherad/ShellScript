using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class CallString : Call
        {
            public override string Name => nameof(CallString);
            public override string Summary { get; }
            public override DataTypes DataType => DataTypes.String;

        }
    }
}