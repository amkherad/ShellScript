using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Diagnostics;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Diagnostics
{
    public partial class BashConsole : ApiConsole
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashWriteLine(),
            new BashWriteError(),
            new BashEnableErrorLog(),
            new BashDisableErrorLog(),
            new BashReadLine(),
            new BashReadText(),
            new BashReadKey(),
            new BashClear(),
            new BashIsTerminal(),
            new BashGetWindowWidth(),
            new BashGetWindowHeight(),
            new BashReadKeyTimeout(),
            new BashIsStdinTerminal(),
            new BashEnableMouseReporting(),
            new BashDisableMouseReporting(),
            new BashReadTerminalInputTimeout(),
            new BashEnterInteractiveInputMode(),
            new BashExitInteractiveInputMode(),
            new BashConsumeInterruptRequest(),
            new BashMoveCursorHome(),
            new BashBeginBatchWrite(),
            new BashEndBatchWrite(),
            new BashWrite(),
            new BashSetCursorPosition(),
        };

        public class BashWriteLine : WriteLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s\\n' \"$1\"");
        }

        public class BashWrite : Write
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s' \"$1\"");
        }

        public class BashSetCursorPosition : SetCursorPosition
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '\\033[%d;%dH' \"$1\" \"$2\"");
        }

        public class BashWriteError : WriteError
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "printf '%s\\n' \"$1\" >&2\n" +
                    "if [ -n \"${__SS_CONSOLE_ERROR_LOG:-}\" ]; then printf '%s\\n' \"$1\" >> \"${__SS_CONSOLE_ERROR_LOG}\"; fi");
        }

        public class BashReadLine : ReadLine
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "IFS= read -r line; printf '%s' \"$line\"");
        }

        public class BashReadText : ReadText
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "IFS= read -r -p \"$1\" line; printf '%s' \"$line\"");
        }

        public class BashReadKey : ReadKey
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [ \"$1\" = \"1\" ]; then IFS= read -r -n 1 key; else IFS= read -r -s -n 1 key; fi; printf '%s' \"$key\"");
        }

        public class BashClear : Clear
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '\\033[2J\\033[H'");
        }

        public class BashMoveCursorHome : MoveCursorHome
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '\\033[H'");
        }

        public class BashBeginBatchWrite : BeginBatchWrite
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [ -z \"${__SS_CONSOLE_BATCH_FILE:-}\" ]; then __SS_CONSOLE_BATCH_FILE=$(mktemp); exec 9>&1; exec 1>>\"$__SS_CONSOLE_BATCH_FILE\"; fi");
        }

        public class BashEndBatchWrite : EndBatchWrite
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [ -n \"${__SS_CONSOLE_BATCH_FILE:-}\" ]; then exec 1>&9; exec 9>&-; cat \"$__SS_CONSOLE_BATCH_FILE\"; rm -f \"$__SS_CONSOLE_BATCH_FILE\"; unset __SS_CONSOLE_BATCH_FILE; fi");
        }

        public class BashIsTerminal : IsTerminal
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashTestCommand.CreateTestExpression(this, p, call, (_, statement) =>
                    new ExpressionResult(TypeDescriptor, "[ -t 1 ]", statement));
        }

        public class BashGetWindowWidth : GetWindowWidth
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "tput cols 2>/dev/null || echo 80");
        }

        public class BashGetWindowHeight : GetWindowHeight
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "tput lines 2>/dev/null || echo 24");
        }

        public class BashReadKeyTimeout : ReadKeyTimeout
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.String, nameof(ReadKeyTimeout), null,
                    ClassAccessName, false, Parameters, null);
                return WriteNativeMethod(this, p,
                    "local ms=\"$1\"\n" +
                    "local t\n" +
                    "t=$(awk -v ms=\"$ms\" 'BEGIN{printf \"%.3f\", ms/1000}')\n" +
                    "local key=\"\"\n" +
                    "if [ \"$2\" = \"1\" ]; then\n" +
                    "  if ! IFS= read -r -n 1 -t \"$t\" key; then printf ''; return 0; fi\n" +
                    "else\n" +
                    "  if ! IFS= read -r -s -n 1 -t \"$t\" key; then printf ''; return 0; fi\n" +
                    "fi\n" +
                    "if [ \"$key\" != $'\\033' ]; then\n" +
                    "  printf '%s' \"$key\"\n" +
                    "  return 0\n" +
                    "fi\n" +
                    "local seq=\"\"\n" +
                    "local c=\"\"\n" +
                    "local i=0\n" +
                    "while [ \"$i\" -lt 12 ]; do\n" +
                    "  if ! IFS= read -r -s -n 1 -t 0.05 c; then\n" +
                    "    if [ -z \"$seq\" ]; then printf 'ESC'; return 0; fi\n" +
                    "    break\n" +
                    "  fi\n" +
                    "  seq=\"${seq}${c}\"\n" +
                    "  case \"$seq\" in \"[A\"|\"[B\"|\"[C\"|\"[D\"|\"OA\"|\"OB\"|\"OC\"|\"OD\") break ;; esac\n" +
                    "  case \"$c\" in '~') break ;; esac\n" +
                    "  i=$((i + 1))\n" +
                    "done\n" +
                    "case \"$seq\" in\n" +
                    "  \"[A\"|\"OA\") printf 'UP'; return 0 ;;\n" +
                    "  \"[B\"|\"OB\") printf 'DOWN'; return 0 ;;\n" +
                    "  \"[D\"|\"OD\") printf 'LEFT'; return 0 ;;\n" +
                    "  \"[C\"|\"OC\") printf 'RIGHT'; return 0 ;;\n" +
                    "esac\n" +
                    "if [[ \"$seq\" =~ ^\\[5.*~$ ]]; then printf 'PAGE_UP'; return 0; fi\n" +
                    "if [[ \"$seq\" =~ ^\\[6.*~$ ]]; then printf 'PAGE_DOWN'; return 0; fi\n" +
                    "if [ -n \"$seq\" ]; then printf 'ESC'; return 0; fi\n" +
                    "printf 'ESC'",
                    info, call.Parameters, call.Info);
            }
        }
    }
}
