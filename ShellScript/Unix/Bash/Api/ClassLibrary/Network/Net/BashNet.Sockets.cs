using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Network.Net;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net
{
    public partial class BashNet
    {
        public class BashGetOpenSockets : GetOpenSockets
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array),
                    nameof(GetOpenSockets), null, ClassAccessName, false, Parameters, null);
                p.Context.GetLastFunctionCallStorageVariable(info.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)\n" +
                    "if [ ${#LastFunctionCall[@]} -gt 0 ]; then\n" +
                    "  mapfile -t LastFunctionCall < <(printf '%s\\n' \"${LastFunctionCall[@]}\" | awk '{\n" +
                    "    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f,\" \"); last=f[n]; if (match(last,/^[0-9]+\\//)) { sub(/\\/.*/,\"\",last); pid=last+0 } }\n" +
                    "    printf \"%010d%s\\n\", pid, $0 }' | sort -n | cut -c11-)\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }

        public class BashGetSocketProcessId : GetSocketProcessId
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(TypeDescriptor.Integer, nameof(GetSocketProcessId), null,
                    ClassAccessName, false, Parameters, null);
                return WriteNativeMethod(this, p,
                    "line=\"$1\"\n" +
                    "if [[ \"$line\" == *pid=* ]]; then\n" +
                    "  rest=${line#*pid=}\n" +
                    "  echo \"${rest%%,*}\"\n" +
                    "else\n" +
                    "  last=\"\"\n" +
                    "  for f in $line; do last=$f; done\n" +
                    "  if [[ \"$last\" == */* ]]; then echo \"${last%%/*}\"; else echo 0; fi\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }
    }
}
