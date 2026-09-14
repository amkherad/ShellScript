using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library.Text;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Text
{
    public partial class BashRegex : ApiRegex
    {
        public override IApiFunc[] Functions { get; } = {new BashIsMatch(), new BashReplace()};

        public class BashIsMatch : IsMatch
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if printf '%s' \"$1\" | grep -Eq -- \"$2\"; then echo 1; else echo 0; fi");
        }

        public class BashReplace : Replace
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "printf '%s' \"$1\" | sed -E \"s|$2|$3|g\"");
        }
    }
}
