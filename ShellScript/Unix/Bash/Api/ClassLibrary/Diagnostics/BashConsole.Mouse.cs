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
        public class BashIsStdinTerminal : IsStdinTerminal
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashTestCommand.CreateTestExpression(this, p, call, (_, statement) =>
                    new ExpressionResult(TypeDescriptor, "[ -t 0 ]", statement));
        }

        public class BashEnableMouseReporting : EnableMouseReporting
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "printf '\\033[?1000h\\033[?1006h'");
        }

        public class BashDisableMouseReporting : DisableMouseReporting
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "printf '\\033[?1006l\\033[?1000l'");
        }

        public class BashReadTerminalInputTimeout : ReadTerminalInputTimeout
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.String, nameof(ReadTerminalInputTimeout), null,
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
                    "local end=\"\"\n" +
                    "local i=0\n" +
                    "while [ \"$i\" -lt 48 ]; do\n" +
                    "  if ! IFS= read -r -s -n 1 -t 0.05 c; then\n" +
                    "    if [ -z \"$seq\" ]; then printf 'ESC'; return 0; fi\n" +
                    "    break\n" +
                    "  fi\n" +
                    "  seq=\"${seq}${c}\"\n" +
                    "  case \"$c\" in M|m) end=\"$c\"; break ;; '~') break ;; esac\n" +
                    "  i=$((i + 1))\n" +
                    "done\n" +
                    "if [ \"${seq:0:2}\" = \"[<\" ]; then\n" +
                    "  local inner=\"${seq:2}\"\n" +
                    "  end=\"${inner: -1}\"\n" +
                    "  inner=\"${inner%M}\"\n" +
                    "  inner=\"${inner%m}\"\n" +
                    "  local btn=\"\" col=\"\" row=\"\"\n" +
                    "  IFS=';' read -r btn col row <<< \"$inner\"\n" +
                    "  if [ \"$btn\" -ge 64 ] 2>/dev/null; then\n" +
                    "    printf 'MOUSE:%s:%s:%s' \"$row\" \"$col\" \"$btn\"\n" +
                    "    return 0\n" +
                    "  fi\n" +
                    "  if [ \"$end\" = \"M\" ] && [ \"$btn\" -eq 0 ]; then\n" +
                    "    printf 'MOUSE:%s:%s:%s' \"$row\" \"$col\" \"$btn\"\n" +
                    "    return 0\n" +
                    "  fi\n" +
                    "fi\n" +
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
