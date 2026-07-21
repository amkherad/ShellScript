using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Environment
{
    public partial class BashEnvironment
    {
        public class BashGetVariable : GetVariable
        {
            private readonly FunctionInfo _functionInfo = new FunctionInfo(TypeDescriptor.String,
                "GetVariable", null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "printf '%s' \"${!1-}\"", _functionInfo,
                    functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
