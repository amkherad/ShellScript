using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Platform;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class BashPlatform
    {
        public class BashGetScriptDirectory : GetScriptDirectory
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "local src=\"${BASH_SOURCE[1]}\"\n" +
                    "if [ -z \"$src\" ]; then src=\"${BASH_SOURCE[0]}\"; fi\n" +
                    "(cd \"$(dirname \"$src\")\" && pwd)");
        }
    }
}
