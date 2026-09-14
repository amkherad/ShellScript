using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Directory
{
    public partial class BashDirectory
    {
        public class BashGetDirectories : GetDirectories
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array),
                    nameof(GetDirectories), null, ClassAccessName, false, Parameters, null);
                p.Context.GetLastFunctionCallStorageVariable(info.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "mapfile -t LastFunctionCall < <(find \"$1\" -maxdepth 1 -mindepth 1 -type d 2>/dev/null | LC_ALL=C sort)",
                    info, call.Parameters, call.Info);
            }
        }

        public class BashCopy : Copy
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "cp -a -- \"$1\" \"$2\"");
        }

        public class BashMove : Move
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "mv -- \"$1\" \"$2\"");
        }
    }
}
