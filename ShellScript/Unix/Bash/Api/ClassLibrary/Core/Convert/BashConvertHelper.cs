using System.Collections.Generic;
using System.Globalization;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Convert;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert
{
    internal static class BashConvertHelper
    {
        private const string ToIntegerBody =
            "awk -v a=\"$1\" 'BEGIN { printf \"%.0f\\n\", a+0 }'";

        private const string ToFloatBody =
            "awk -v a=\"$1\" 'BEGIN { print a+0 }'";

        private const string ToBooleanBody =
            "local v\n" +
            "v=$(printf '%s' \"$1\" | tr '[:upper:]' '[:lower:]')\n" +
            "case \"$v\" in\n" +
            "  ''|0|false|no|off) echo 0 ;;\n" +
            "  *) echo 1 ;;\n" +
            "esac";

        private static readonly Dictionary<string, string> IntegerUtilityBodies =
            BashMathUtilityBodies.Unary("printf \"%.0f\\n\", a+0", "($1)/1", "print(int(float(a)))");

        private static readonly Dictionary<string, string> FloatUtilityBodies =
            BashMathUtilityBodies.Unary("print a+0", "$1", "print(float(a))");

        public static IApiMethodBuilderResult BuildToInteger(ApiConvert.ToInteger func, ExpressionBuilderParams p,
            FunctionCallStatement call)
        {
            return BuildCoercion(func, p, call, TypeDescriptor.Integer, nameof(ApiConvert.ToInteger), IntegerUtilityBodies,
                ToIntegerBody, "echo $(( ${1%.*} ))");
        }

        public static IApiMethodBuilderResult BuildToFloat(ApiConvert.ToFloat func, ExpressionBuilderParams p,
            FunctionCallStatement call)
        {
            return BuildCoercion(func, p, call, TypeDescriptor.Float, nameof(ApiConvert.ToFloat), FloatUtilityBodies, ToFloatBody,
                "printf '%s' \"$1\"");
        }

        public static IApiMethodBuilderResult BuildToNumber(ApiConvert.ToNumber func, ExpressionBuilderParams p,
            FunctionCallStatement call)
        {
            return BuildCoercion(func, p, call, TypeDescriptor.Numeric, nameof(ApiConvert.ToNumber), FloatUtilityBodies,
                ToFloatBody, "printf '%s' \"$1\"");
        }

        public static IApiMethodBuilderResult BuildToBoolean(ApiConvert.ToBoolean func, ExpressionBuilderParams p,
            FunctionCallStatement call)
        {
            func.AssertParameters(p, call.Parameters);
            var param = call.Parameters[0];

            if (param is ConstantValueStatement constant)
            {
                return BuildBooleanConstant(constant);
            }

            if (param is VariableAccessStatement variableAccess &&
                p.Scope.TryGetVariableInfo(variableAccess, out var varInfo))
            {
                if (varInfo.TypeDescriptor.IsBoolean())
                {
                    return new ApiMethodBuilderRawResult(new ExpressionResult(
                        TypeDescriptor.Boolean, $"${varInfo.AccessName}", variableAccess));
                }

                if (varInfo.TypeDescriptor.IsInteger() || varInfo.TypeDescriptor.IsNumericOrFloat())
                {
                    return new ApiMethodBuilderRawResult(new ExpressionResult(
                        TypeDescriptor.Boolean, $"$(( ${varInfo.AccessName} != 0 ))", variableAccess));
                }
            }

            var functionInfo = new FunctionInfo(TypeDescriptor.Boolean, nameof(ApiConvert.ToBoolean), null,
                ApiConvert.ClassAccessName, false, func.Parameters, null);

            return ApiBaseFunction.WriteNativeMethod(func, p, ToBooleanBody, functionInfo, call.Parameters, call.Info);
        }

        public static IApiMethodBuilderResult BuildParse(ApiConvert.Parse func, ExpressionBuilderParams p,
            FunctionCallStatement call)
        {
            func.AssertParameters(p, call.Parameters);
            var functionInfo = new FunctionInfo(TypeDescriptor.Numeric, nameof(ApiConvert.Parse), null,
                ApiConvert.ClassAccessName, false, func.Parameters, null);

            return ApiBaseFunction.CreateNativeMethodWithUtilityExpressionSelector(func, p, functionInfo, FloatUtilityBodies,
                call.Parameters, call.Info, ToFloatBody);
        }

        private static IApiMethodBuilderResult BuildCoercion(
            ApiBaseFunction func,
            ExpressionBuilderParams p,
            FunctionCallStatement call,
            TypeDescriptor targetType,
            string nativeName,
            IDictionary<string, string> utilityBodies,
            string utilityFallbackBody,
            string pureBashFallback)
        {
            func.AssertParameters(p, call.Parameters);
            var param = call.Parameters[0];

            if (param is ConstantValueStatement constant)
            {
                return BuildNumericConstant(constant, targetType);
            }

            if (param is VariableAccessStatement variableAccess)
            {
                if (p.Scope.TryGetVariableInfo(variableAccess, out var varInfo))
                {
                    if (varInfo.TypeDescriptor == targetType ||
                        (targetType.IsNumericOrFloat() && varInfo.TypeDescriptor.IsNumericOrFloat()) ||
                        (targetType.IsInteger() && varInfo.TypeDescriptor.IsInteger()))
                    {
                        return new ApiMethodBuilderRawResult(new ExpressionResult(
                            targetType, $"${varInfo.AccessName}", variableAccess));
                    }
                }

                if (p.Scope.TryGetConstantInfo(variableAccess, out var constInfo))
                {
                    return BuildNumericConstant(
                        new ConstantValueStatement(constInfo.TypeDescriptor, constInfo.Value, variableAccess.Info),
                        targetType);
                }
            }

            var functionInfo = new FunctionInfo(targetType, nativeName, null, ApiConvert.ClassAccessName, false,
                func.Parameters, null);

            return ApiBaseFunction.CreateNativeMethodWithUtilityExpressionSelector(func, p, functionInfo, utilityBodies,
                call.Parameters, call.Info, pureBashFallback ?? utilityFallbackBody);
        }

        private static IApiMethodBuilderResult BuildNumericConstant(ConstantValueStatement constant,
            TypeDescriptor targetType)
        {
            if (constant.TypeDescriptor.IsNumber())
            {
                if (targetType.IsInteger() &&
                    long.TryParse(constant.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo,
                        out var integer))
                {
                    return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Integer,
                        integer.ToString(NumberFormatInfo.InvariantInfo), constant.Info));
                }

                if (double.TryParse(constant.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo,
                        out var number))
                {
                    if (targetType.IsInteger())
                    {
                        return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Integer,
                            ((long) number).ToString(NumberFormatInfo.InvariantInfo), constant.Info));
                    }

                    return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Float,
                        number.ToString(NumberFormatInfo.InvariantInfo), constant.Info));
                }
            }

            if (constant.TypeDescriptor.IsString() &&
                double.TryParse(constant.Value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var parsed))
            {
                if (targetType.IsInteger())
                {
                    return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Integer,
                        ((long) parsed).ToString(NumberFormatInfo.InvariantInfo), constant.Info));
                }

                return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Float,
                    parsed.ToString(NumberFormatInfo.InvariantInfo), constant.Info));
            }

            throw new TypeMismatchCompilerException(constant.TypeDescriptor, targetType, constant.Info);
        }

        private static IApiMethodBuilderResult BuildBooleanConstant(ConstantValueStatement constant)
        {
            if (constant.TypeDescriptor.IsBoolean())
            {
                return ApiBaseFunction.Inline(constant);
            }

            if (constant.TypeDescriptor.IsNumber() &&
                long.TryParse(constant.Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var number))
            {
                return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Boolean,
                    (number != 0).ToString(NumberFormatInfo.InvariantInfo), constant.Info));
            }

            if (constant.TypeDescriptor.IsString())
            {
                var value = constant.Value.Trim().ToLowerInvariant();
                var truthy = value.Length > 0 && value != "0" && value != "false" && value != "no" && value != "off";
                return ApiBaseFunction.Inline(new ConstantValueStatement(TypeDescriptor.Boolean,
                    truthy.ToString(NumberFormatInfo.InvariantInfo), constant.Info));
            }

            throw new TypeMismatchCompilerException(constant.TypeDescriptor, TypeDescriptor.Boolean, constant.Info);
        }
    }
}
