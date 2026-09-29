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
        public class BashEnterInteractiveInputMode : EnterInteractiveInputMode
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.Void, nameof(EnterInteractiveInputMode), null,
                    ClassAccessName, false, Parameters, null);
                return WriteNativeMethod(this, p,
                    "if [ -t 0 ] && [ -z \"${__SS_CONSOLE_STTY_SAVED:-}\" ]; then\n" +
                    "  __SS_CONSOLE_STTY_SAVED=$(stty -g 2>/dev/null) || return 0\n" +
                    "  stty -echo -icanon min 0 time 0 2>/dev/null || stty -echo -icanon 2>/dev/null\n" +
                    "  trap 'Console_ExitInteractiveInputMode' EXIT HUP\n" +
                    "  trap 'Console_ExitInteractiveInputMode; __SS_CONSOLE_INTERRUPT=1' INT TERM\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }

        public class BashExitInteractiveInputMode : ExitInteractiveInputMode
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.Void, nameof(ExitInteractiveInputMode), null,
                    ClassAccessName, false, Parameters, null);
                return WriteNativeMethod(this, p,
                    "if [ -n \"${__SS_CONSOLE_STTY_SAVED:-}\" ]; then\n" +
                    "  stty \"$__SS_CONSOLE_STTY_SAVED\" 2>/dev/null || true\n" +
                    "  unset __SS_CONSOLE_STTY_SAVED\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }

        public class BashConsumeInterruptRequest : ConsumeInterruptRequest
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.Boolean, nameof(ConsumeInterruptRequest), null,
                    ClassAccessName, false, Parameters, null);
                return WriteNativeMethod(this, p,
                    "if [ \"${__SS_CONSOLE_INTERRUPT:-0}\" = \"1\" ]; then\n" +
                    "  __SS_CONSOLE_INTERRUPT=0\n" +
                    "  echo 1\n" +
                    "else\n" +
                    "  echo 0\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }
    }
}
