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
        public class BashGetNetworkInterfaces : GetNetworkInterfaces
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array),
                    nameof(GetNetworkInterfaces), null, ClassAccessName, false, Parameters, null);
                p.Context.GetLastFunctionCallStorageVariable(info.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "if command -v ip >/dev/null; then\n" +
                    "  mapfile -t LastFunctionCall < <(ip -o link show 2>/dev/null | awk -F': ' '{n=$2; sub(/@.*/,\"\",n); print n}')\n" +
                    "else\n" +
                    "  mapfile -t LastFunctionCall < <(ls /sys/class/net 2>/dev/null)\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }

        public class BashGetOpenSocketsOnInterface : GetOpenSocketsOnInterface
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array),
                    nameof(GetOpenSocketsOnInterface), null, ClassAccessName, false, Parameters, null);
                p.Context.GetLastFunctionCallStorageVariable(info.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "if [ -z \"$1\" ]; then\n" +
                    "  mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)\n" +
                    "elif command -v ss >/dev/null; then\n" +
                    "  mapfile -t LastFunctionCall < <(ss -H -antup dev \"$1\" 2>/dev/null)\n" +
                    "else\n" +
                    "  mapfile -t LastFunctionCall < <(netstat -antup -i 2>/dev/null | tail -n +3 | awk -v d=\"$1\" '$0 ~ d {print}')\n" +
                    "fi\n" +
                    "if [ ${#LastFunctionCall[@]} -gt 0 ]; then\n" +
                    "  mapfile -t LastFunctionCall < <(printf '%s\\n' \"${LastFunctionCall[@]}\" | awk '{\n" +
                    "    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f,\" \"); last=f[n]; if (match(last,/^[0-9]+\\//)) { sub(/\\/.*/,\"\",last); pid=last+0 } }\n" +
                    "    printf \"%010d%s\\n\", pid, $0 }' | sort -n | cut -c11-)\n" +
                    "fi",
                    info, call.Parameters, call.Info);
            }
        }
    }
}
