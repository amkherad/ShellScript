using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Data.Ini;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Data.Ini
{
    public partial class BashIni : ApiIni
    {
        public override IApiFunc[] Functions { get; } = {new BashGetValue(), new BashSetValue()};

        public class BashGetValue : GetValue
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "awk -F= -v section=\"[$2]\" -v key=\"$3\" '\n" +
                    "  $0 ~ /^[[:space:]]*#/ { next }\n" +
                    "  $0 ~ /^[[:space:]]*\\[/ { in_section = ($0 == section); next }\n" +
                    "  in_section && $1 == key { sub(/^[^=]*=/, \"\"); print; exit }\n" +
                    "' \"$1\"");
        }

        public class BashSetValue : SetValue
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "python3 -c 'import sys,re,pathlib; p,sec,key,val=sys.argv[1:5]; t=pathlib.Path(p); lines=t.read_text().splitlines() if t.exists() else []; out=[]; in_s=False; found=False; sec_h=\"[\"+sec+\"]\";\n" +
                    "for line in lines:\n" +
                    "  if line.strip().startswith(\"[\") and line.strip().endswith(\"]\"):\n" +
                    "    if in_s and not found: out.append(key+\"=\"+val); found=True\n" +
                    "    in_s=line.strip()==sec_h; out.append(line); continue\n" +
                    "  if in_s and line.split(\"=\",1)[0].strip()==key: out.append(key+\"=\"+val); found=True; continue\n" +
                    "  out.append(line)\n" +
                    "if not any(l.strip()==sec_h for l in out): out.append(sec_h)\n" +
                    "if not found: out.append(key+\"=\"+val)\n" +
                    "t.write_text(\"\\n\".join(out)+\"\\n\")' \"$1\" \"$2\" \"$3\" \"$4\"");
        }
    }
}
