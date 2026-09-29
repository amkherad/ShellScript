using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class BashString
    {
        public class BashJoin : Join
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(Join), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                var arrayName = BashArrayArgumentHelpers.ResolveArrayVariableName(p, call.Parameters[0], call);
                var arrayNameParam = new ConstantValueStatement(TypeDescriptor.String, arrayName, call.Info);
                return WriteNativeMethod(this, p,
                    "local n=\"$1\" sep=\"$2\" first=1 out=\"\"\n" +
                    "eval 'for part in \"${'\"$n\"'[@]}\"; do " +
                    "if [ $first -eq 1 ]; then out=\"$part\"; first=0; else out=\"${out}${sep}${part}\"; fi; " +
                    "done'\n" +
                    "printf '%s' \"$out\"",
                    FunctionInfo, new[] {arrayNameParam, call.Parameters[1]}, call.Info);
            }
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

        public class BashRepeat : Repeat
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(Repeat), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local i=0 out=\"\"\n" +
                    "while [ $i -lt $2 ]; do out=\"${out}$1\"; i=$((i + 1)); done\n" +
                    "printf '%s' \"$out\"",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashTrimStart : TrimStart
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(TrimStart), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local s=\"$1\"\n" +
                    "s=\"${s#\"${s%%[![:space:]]*}\"}\"\n" +
                    "printf '%s' \"$s\"",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashTrimEnd : TrimEnd
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(TrimEnd), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local s=\"$1\"\n" +
                    "s=\"${s%\"${s##*[![:space:]]}\"}\"\n" +
                    "printf '%s' \"$s\"",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashPadLeft : PadLeft
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(PadLeft), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local s=\"$1\" w=$2 pad=\"$3\"\n" +
                    "while [ ${#s} -lt $w ]; do s=\"${pad}${s}\"; done\n" +
                    "printf '%s' \"$s\"",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashPadRight : PadRight
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(PadRight), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local s=\"$1\" w=$2 pad=\"$3\"\n" +
                    "while [ ${#s} -lt $w ]; do s=\"${s}${pad}\"; done\n" +
                    "printf '%s' \"$s\"",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashLastIndexOf : LastIndexOf
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.Integer, nameof(LastIndexOf), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local hay=\"$1\" needle=\"$2\" suffix\n" +
                    "suffix=${hay##*\"$needle\"}\n" +
                    "if [ \"$suffix\" = \"$hay\" ]; then echo -1; else echo $((${#hay} - ${#suffix} - ${#needle})); fi",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashCompare : Compare
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.Integer, nameof(Compare), null,
                ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "if [[ \"$1\" < \"$2\" ]]; then echo -1; elif [[ \"$1\" > \"$2\" ]]; then echo 1; else echo 0; fi",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashCompareIgnoreCase : CompareIgnoreCase
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.Integer, nameof(CompareIgnoreCase),
                null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                WriteNativeMethod(this, p,
                    "local a=$(printf '%s' \"$1\" | tr '[:upper:]' '[:lower:]')\n" +
                    "local b=$(printf '%s' \"$2\" | tr '[:upper:]' '[:lower:]')\n" +
                    "if [[ \"$a\" < \"$b\" ]]; then echo -1; elif [[ \"$a\" > \"$b\" ]]; then echo 1; else echo 0; fi",
                    FunctionInfo, call.Parameters, call.Info);
        }

        public class BashContainsIgnoreCase : ContainsIgnoreCase
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashTestCommand.CreateTestExpression(this, p, call, (parameters, statement) =>
                {
                    var usage = parameters.UsageContext ?? call;
                    var leftTranspiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[0]);
                    var rightTranspiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[1]);
                    var left = leftTranspiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, usage, call.Parameters[0]);
                    var right = rightTranspiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, usage, call.Parameters[1]);
                    return new ExpressionResult(TypeDescriptor,
                        $"[[ `String_ToLower {left.Expression}` == *`String_ToLower {right.Expression}`* ]]",
                        call);
                });
        }
    }
}
