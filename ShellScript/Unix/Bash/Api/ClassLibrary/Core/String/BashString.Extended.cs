using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class BashString
    {
        public class BashToLower : ToLower
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(ToLower), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "printf '%s' \"$(printf '%s' \"$1\" | tr '[:upper:]' '[:lower:]')\"",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashToUpper : ToUpper
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(ToUpper), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p, "printf '%s' \"$(printf '%s' \"$1\" | tr '[:lower:]' '[:upper:]')\"",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashTrim : Trim
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(Trim), null, ClassAccessName, false, Parameters, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "local s=\"$1\"\n" +
                    "s=\"${s#\"${s%%[![:space:]]*}\"}\"\n" +
                    "s=\"${s%\"${s##*[![:space:]]}\"}\"\n" +
                    "printf '%s' \"$s\"",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashGetBefore : GetBefore
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(GetBefore), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "case \"$1\" in *\"$2\"*) printf '%s' \"${1%%\"$2\"*}\" ;; *) printf '%s' \"$1\" ;; esac",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashGetAfter : GetAfter
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(GetAfter), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "case \"$1\" in *\"$2\"*) printf '%s' \"${1#*\"$2\"}\" ;; *) printf '' ;; esac",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashIndexOf : IndexOf
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.Integer, nameof(IndexOf), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "local hay=\"$1\" needle=\"$2\" prefix\n" +
                    "prefix=${hay%%\"$needle\"*}\n" +
                    "if [ \"$prefix\" = \"$hay\" ]; then echo -1; else echo ${#prefix}; fi",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashSubstring : Substring
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(Substring), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "if [ -z \"$3\" ] || [ \"$3\" -lt 0 ]; then printf '%s' \"${1:$2}\"; else printf '%s' \"${1:$2:$3}\"; fi",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }

        public class BashEquals : Equals
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return BashTestCommand.CreateTestExpression(this, p, functionCallStatement, (parameters, call) =>
                {
                    var leftTranspiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[0]);
                    var rightTranspiler = parameters.Context.GetEvaluationTranspilerForStatement(call.Parameters[1]);
                    var left = leftTranspiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, call.Parameters[0]);
                    var right = rightTranspiler.GetExpression(parameters.Context, parameters.Scope,
                        parameters.MetaWriter, parameters.NonInlinePartWriter, call, call.Parameters[1]);
                    var leftExpr = FormatEqualsOperand(call.Parameters[0], left.Expression);
                    var rightExpr = FormatEqualsOperand(call.Parameters[1], right.Expression);
                    return new ExpressionResult(TypeDescriptor, $"[[ {leftExpr} == {rightExpr} ]]", call);
                });
            }

            private static string FormatEqualsOperand(EvaluationStatement parameter, string expression)
            {
                if (parameter is ConstantValueStatement constant && constant.IsString())
                {
                    return BashTranspilerHelpers.ToBashStringLiteral(constant.Value, true);
                }

                return expression;
            }
        }

        public class BashReplace : Replace
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.String, nameof(Replace), null, ClassAccessName, false, Parameters,
                    null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return WriteNativeMethod(this, p,
                    "printf '%s' \"${1//$2/$3}\"",
                    FunctionInfo, functionCallStatement.Parameters, functionCallStatement.Info);
            }
        }
    }
}
