using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class BashPlatform
    {
        public class BashCallArray : CallArray
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array), nameof(CallArray), null,
                    ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                p.Context.GetLastFunctionCallStorageVariable(FunctionInfo.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "mapfile -t LastFunctionCall < <(eval \"$1\")",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
