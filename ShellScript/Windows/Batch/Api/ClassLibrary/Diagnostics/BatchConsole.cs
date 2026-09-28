using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Diagnostics;
using ShellScript.Windows.Batch.Api.ClassLibrary.Base;

namespace ShellScript.Windows.Batch.Api.ClassLibrary.Diagnostics
{
    public partial class BatchConsole : ApiConsole
    {
        public override IApiFunc[] Functions { get; } = {new BatchWriteLine(), new BatchWriteError()};

        public class BatchWriteLine : WriteLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BatchApiNative.Native(this, p, call, ClassAccessName, "echo %~1");
        }

        public class BatchWriteError : WriteError
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BatchApiNative.Native(this, p, call, ClassAccessName, "echo %~1 1>&2");
        }
    }
}
