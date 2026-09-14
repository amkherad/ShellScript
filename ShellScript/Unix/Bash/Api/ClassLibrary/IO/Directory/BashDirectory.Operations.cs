using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.Directory
{
    public partial class BashDirectory
    {
        public class BashExists : Exists
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, "d");
            }
        }

        public class BashCreate : Create
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(Create), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "mkdir -p -- \"$1\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashDelete : Delete
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(Delete), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "rmdir -- \"$1\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashDeleteRecursive : DeleteRecursive
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(DeleteRecursive), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "rm -rf -- \"$1\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashGetFiles : GetFiles
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(new TypeDescriptor(DataTypes.String | DataTypes.Array), nameof(GetFiles), null,
                    ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                p.Context.GetLastFunctionCallStorageVariable(_functionInfo.TypeDescriptor, p.MetaWriter);
                return WriteNativeMethod(this, p,
                    "mapfile -t LastFunctionCall < <(find \"$1\" -maxdepth 1 -type f 2>/dev/null | LC_ALL=C sort)",
                    _functionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
