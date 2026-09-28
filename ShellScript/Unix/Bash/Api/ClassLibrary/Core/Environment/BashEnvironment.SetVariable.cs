using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Environment
{
    public partial class BashEnvironment
    {
        public class BashSetVariable : SetVariable
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.Void, nameof(SetVariable), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "export \"$1=$2\"", FunctionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }
    }
}
