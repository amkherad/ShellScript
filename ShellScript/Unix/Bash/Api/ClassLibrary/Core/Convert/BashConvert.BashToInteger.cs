using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert
{
    public partial class BashConvert
    {
        public class BashToInteger : ToInteger
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BashConvertHelper.BuildToInteger(this, p, functionCallStatement);
        }
    }
}
