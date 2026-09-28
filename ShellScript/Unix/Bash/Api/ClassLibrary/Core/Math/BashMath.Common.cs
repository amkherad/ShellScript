using System.Collections.Generic;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class BashMath
    {
        private static IApiMethodBuilderResult BuildUnaryNumericMath(
            ApiBaseFunction func,
            ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement,
            string nativeName,
            FunctionParameterDefinitionStatement[] parameters,
            IDictionary<string, string> utilityBodies,
            string pureBashFallback)
        {
            func.AssertParameters(p, functionCallStatement.Parameters);
            var functionInfo = new FunctionInfo(TypeDescriptor.Numeric, nativeName, null, ClassAccessName, false,
                parameters, null);
            return ApiBaseFunction.CreateNativeMethodWithUtilityExpressionSelector(func, p, functionInfo, utilityBodies,
                functionCallStatement.Parameters, functionCallStatement.Info, pureBashFallback);
        }

        private static IApiMethodBuilderResult BuildBinaryNumericMath(
            ApiBaseFunction func,
            ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement,
            string nativeName,
            FunctionParameterDefinitionStatement[] parameters,
            IDictionary<string, string> utilityBodies,
            string pureBashFallback)
        {
            func.AssertParameters(p, functionCallStatement.Parameters);
            var functionInfo = new FunctionInfo(TypeDescriptor.Numeric, nativeName, null, ClassAccessName, false,
                parameters, null);
            return ApiBaseFunction.CreateNativeMethodWithUtilityExpressionSelector(func, p, functionInfo, utilityBodies,
                functionCallStatement.Parameters, functionCallStatement.Info, pureBashFallback);
        }

        public class BashMin : Min
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildBinaryNumericMath(this, p, functionCallStatement, nameof(Min), Parameters,
                    BashMathUtilityBodies.Binary(
                        "print (a < b ? a : b)",
                        "if ($1<$2) $1 else $2",
                        "print(min(a,b))"),
                    "if [ \"$1\" -lt \"$2\" ]; then echo \"$1\"; else echo \"$2\"; fi");
        }

        public class BashMax : Max
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildBinaryNumericMath(this, p, functionCallStatement, nameof(Max), Parameters,
                    BashMathUtilityBodies.Binary(
                        "print (a > b ? a : b)",
                        "if ($1>$2) $1 else $2",
                        "print(max(a,b))"),
                    "if [ \"$1\" -gt \"$2\" ]; then echo \"$1\"; else echo \"$2\"; fi");
        }

        public class BashFloor : Floor
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildUnaryNumericMath(this, p, functionCallStatement, nameof(Floor), Parameters,
                    BashMathUtilityBodies.Unary(
                        "print int(a)",
                        "($1)/1",
                        "print(math.floor(a))"),
                    "echo \"$1\" | awk '{print int($1)}'");
        }

        public class BashCeiling : Ceiling
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildUnaryNumericMath(this, p, functionCallStatement, nameof(Ceiling), Parameters,
                    BashMathUtilityBodies.Unary(
                        "print (a == int(a) ? a : int(a) + 1)",
                        "($1+0.999999)/1",
                        "print(math.ceil(a))"),
                    "echo \"$1\" | awk '{x=$1; print (x==int(x)?x:int(x)+1)}'");
        }

        public class BashRound : Round
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildUnaryNumericMath(this, p, functionCallStatement, nameof(Round), Parameters,
                    BashMathUtilityBodies.Unary(
                        "print int(a + 0.5 * (a < 0 ? -1 : 1))",
                        "($1+0.5)/1",
                        "print(int(round(a)))"),
                    "echo \"$1\" | awk '{printf \"%.0f\\n\", $1}'");
        }

        public class BashTruncate : Truncate
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildUnaryNumericMath(this, p, functionCallStatement, nameof(Truncate), Parameters,
                    BashMathUtilityBodies.Unary(
                        "print int(a)",
                        "($1)/1",
                        "print(int(a) if a >= 0 else -int(-a))"),
                    "echo \"$1\" | awk '{print int($1)}'");
        }

        public class BashSqrt : Sqrt
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildUnaryNumericMath(this, p, functionCallStatement, nameof(Sqrt), Parameters,
                    BashMathUtilityBodies.Unary(
                        "print sqrt(a)",
                        "sqrt($1)",
                        "print(math.sqrt(a))"),
                    "echo \"scale=10; sqrt($1)\" | bc -l 2>/dev/null || echo 0");
        }

        public class BashPow : Pow
        {
            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement) =>
                BuildBinaryNumericMath(this, p, functionCallStatement, nameof(Pow), Parameters,
                    BashMathUtilityBodies.Binary(
                        "print a ^ b",
                        "$1 ^ $2",
                        "print(a ** b)"),
                    "echo \"scale=10; $1 ^ $2\" | bc -l 2>/dev/null || echo 0");
        }

        public class BashSign : Sign
        {
            private FunctionInfo FunctionInfo => new FunctionInfo(TypeDescriptor.Integer, nameof(Sign), null, ClassAccessName, false,
                    new FunctionParameterDefinitionStatement[] {NumberParameter}, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, FunctionInfo,
                    BashMathUtilityBodies.Unary(
                        "if (a < 0) print -1; else if (a > 0) print 1; else print 0",
                        "if ($1<0) -1 else if ($1>0) 1 else 0",
                        "print((a>0)-(a<0))"),
                    functionCallStatement.Parameters, functionCallStatement.Info,
                    "if [ \"$1\" -lt 0 ]; then echo -1; elif [ \"$1\" -gt 0 ]; then echo 1; else echo 0; fi");
            }
        }
    }
}
