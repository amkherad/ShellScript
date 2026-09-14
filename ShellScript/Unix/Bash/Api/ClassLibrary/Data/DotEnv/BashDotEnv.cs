using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Data.DotEnv;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Data.DotEnv
{
    public partial class BashDotEnv : ApiDotEnv
    {
        public override IApiFunc[] Functions { get; } = {new BashLoad(), new BashGet()};

        public class BashLoad : Load
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "set -a\n" +
                    "while IFS= read -r line || [ -n \"$line\" ]; do\n" +
                    "  case \"$line\" in ''|'#'*) continue ;; esac\n" +
                    "  export \"$line\"\n" +
                    "done < \"$1\"\n" +
                    "set +a");
        }

        public class BashGet : Get
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "grep -E \"^$2=\" \"$1\" | tail -n1 | cut -d= -f2-");
        }
    }
}
