using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert
{
    public partial class BashConvert
    {
        public class BashToBoolean : ToBoolean
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BashConvertHelper.BuildToBoolean(this, p, functionCallStatement);
        }
    }
}
