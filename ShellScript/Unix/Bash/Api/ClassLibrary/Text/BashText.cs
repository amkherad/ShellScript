using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Text;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Text
{
    public partial class BashText : ApiText
    {
        public override IApiFunc[] Functions { get; } = {new BashNormalizeWhitespace(), new BashSplit()};

        public class BashNormalizeWhitespace : NormalizeWhitespace
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "printf '%s' \"$1\" | tr -s '[:space:]' ' ' | sed 's/^ //;s/ $//'");
        }

        public class BashSplit : Split
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array), nameof(Split),
                    null, ClassAccessName, false, Parameters, null);
                p.Context.GetLastFunctionCallStorageVariable(info.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "IFS=\"$2\" read -r -a LastFunctionCall <<< \"$1\"",
                    info, call.Parameters, call.Info);
            }
        }
    }
}
