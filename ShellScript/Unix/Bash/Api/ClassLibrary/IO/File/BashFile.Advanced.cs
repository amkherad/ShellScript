using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class BashFile
    {
        public class BashReadAllLines : ReadAllLines
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var info = new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array), nameof(ReadAllLines),
                    null, ClassAccessName, false, Parameters, null);
                p.Context.GetLastFunctionCallStorageVariable(info.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p, "mapfile -t LastFunctionCall < \"$1\"",
                    info, call.Parameters, call.Info);
            }
        }

        public class BashWriteAllLines : WriteAllLines
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, ": > \"$1\"; printf '%s\\n' \"${@:2}\" > \"$1\"");
        }

        public class BashReadAllBytesBase64 : ReadAllBytesBase64
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "base64 -w 0 -- \"$1\" 2>/dev/null || base64 \"$1\"");
        }

        public class BashWriteAllBytesBase64 : WriteAllBytesBase64
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "printf '%s' \"$2\" | base64 -d > \"$1\"");
        }

        public class BashGetLastWriteTime : GetLastWriteTime
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "stat --format=%Y -- \"$1\" 2>/dev/null || stat -f %m -- \"$1\"");
        }

        public class BashCreateSymbolicLink : CreateSymbolicLink
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName, "ln -sf -- \"$2\" \"$1\"");
        }
    }
}
