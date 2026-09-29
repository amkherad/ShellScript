using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Diagnostics;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Diagnostics
{
    public partial class BashConsole
    {
        public class BashEnableErrorLog : EnableErrorLog
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [ \"$2\" = \"1\" ]; then\n" +
                    "  : >> \"$1\"\n" +
                    "else\n" +
                    "  : > \"$1\"\n" +
                    "fi\n" +
                    "__SS_CONSOLE_ERROR_LOG=\"$1\"");
        }

        public class BashDisableErrorLog : DisableErrorLog
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "unset __SS_CONSOLE_ERROR_LOG");
        }
    }
}
