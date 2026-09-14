using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class BashFile
    {
        public class BashReadAllText : ReadAllText
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.String, nameof(ReadAllText), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "cat -- \"$1\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashWriteAllText : WriteAllText
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(WriteAllText), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "printf '%s' \"$2\" > \"$1\"", _functionInfo,
                    functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashAppendAllText : AppendAllText
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(AppendAllText), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "printf '%s' \"$2\" >> \"$1\"", _functionInfo,
                    functionCallStatement.Parameters, functionCallStatement.Info);
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
                return WriteNativeMethod(this, p, "rm -f -- \"$1\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashCopy : Copy
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(Copy), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "cp -- \"$1\" \"$2\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashMove : Move
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Void, nameof(Move), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "mv -- \"$1\" \"$2\"", _functionInfo, functionCallStatement.Parameters,
                    functionCallStatement.Info);
            }
        }

        public class BashGetLength : GetLength
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Integer, nameof(GetLength), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "stat --format=%s -- \"$1\" 2>/dev/null || stat -f%z -- \"$1\"",
                    _functionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
