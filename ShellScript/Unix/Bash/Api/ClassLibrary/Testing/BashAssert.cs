using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Testing;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;
using ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Testing
{
    public partial class BashAssert : ApiAssert
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashEquals(),
            new BashNotEquals(),
            new BashTrue(),
            new BashFalse(),
            new BashFail(),
        };

        public class BashEquals : Equals
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [[ \"$1\" == \"$2\" ]]; then return 0; fi\n" +
                    "if [ -n \"$3\" ]; then printf '%s\\n' \"$3\" >&2; else " +
                    "printf 'Assert.Equals failed: expected \"%s\" but was \"%s\"\\n' \"$1\" \"$2\" >&2; fi\n" +
                    "return 1 2>/dev/null || exit 1");
        }

        public class BashNotEquals : NotEquals
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "if [[ \"$1\" != \"$2\" ]]; then return 0; fi\n" +
                    "if [ -n \"$3\" ]; then printf '%s\\n' \"$3\" >&2; else " +
                    "printf 'Assert.NotEquals failed: both values were \"%s\"\\n' \"$1\" >&2; fi\n" +
                    "return 1 2>/dev/null || exit 1");
        }

        public class BashTrue : True
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor.Void,
                    BuildConditionAssertBlock(p, call, true, "Assert.True failed."),
                    call));
            }
        }

        public class BashFalse : False
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call)
            {
                AssertParameters(p, call.Parameters);
                return new ApiMethodBuilderRawResult(new ExpressionResult(
                    TypeDescriptor.Void,
                    BuildConditionAssertBlock(p, call, false, "Assert.False failed."),
                    call));
            }
        }

        public class BashFail : Fail
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p, FunctionCallStatement call) =>
                BashApiNative.Native(this, p, call, ClassAccessName,
                    "printf '%s\\n' \"$1\" >&2\nreturn 1 2>/dev/null || exit 1");
        }

        private static string BuildConditionAssertBlock(
            ExpressionBuilderParams p, FunctionCallStatement call, bool expectTrue, string defaultMessage)
        {
            var conditionExpr = FormatAssertCondition(p, call.Parameters[0]);
            var passExpr = expectTrue ? conditionExpr : $"! {conditionExpr}";

            var failureMessage = GetFailureMessageExpression(p, call, 1, defaultMessage);
            return $"if {passExpr}; then :; else {failureMessage}; return 1 2>/dev/null || exit 1; fi";
        }

        private static string FormatAssertCondition(ExpressionBuilderParams p, EvaluationStatement statement)
        {
            var builder = BashConditionalExpressionBuilder.Instance;
            var condition = builder.CreateExpression(p, statement);
            var expr = builder.FormatExpression(p, condition);
            if (expr == "[ 0 ]")
            {
                return "[ 1 -eq 0 ]";
            }

            if (expr == "[ 1 ]")
            {
                return "[ 1 -ne 0 ]";
            }

            return expr;
        }

        private static string GetFailureMessageExpression(
            ExpressionBuilderParams p, FunctionCallStatement call, int messageParameterIndex, string defaultMessage)
        {
            if (call.Parameters.Length <= messageParameterIndex)
            {
                return $"printf '%s\\n' \"{EscapeBashSingleQuoted(defaultMessage)}\" >&2";
            }

            var messageParam = call.Parameters[messageParameterIndex];
            if (messageParam is ConstantValueStatement constant &&
                constant.TypeDescriptor.IsString() &&
                string.IsNullOrEmpty(constant.Value))
            {
                return $"printf '%s\\n' \"{EscapeBashSingleQuoted(defaultMessage)}\" >&2";
            }

            var builder = BashDefaultExpressionBuilder.Instance;
            var message = builder.CreateExpression(p, messageParam);
            if (message.Template is ConstantValueStatement || message.Template is VariableAccessStatement)
            {
                return $"printf '%s\\n' {message.Expression} >&2";
            }

            return $"printf '%s\\n' \"{message.Expression}\" >&2";
        }

        private static string EscapeBashSingleQuoted(string value) =>
            value?.Replace("'", "'\\''") ?? string.Empty;
    }
}
