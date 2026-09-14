using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class BashMath
    {
        private abstract class BashUnaryNumericMathFunction<TApi> : TApi where TApi : ApiBaseFunction
        {
            private readonly FunctionInfo _functionInfo;
            private readonly System.Collections.Generic.Dictionary<string, string> _utilityBodies;
            private readonly string _pureBashFallback;

            protected BashUnaryNumericMathFunction(
                string nativeName,
                System.Collections.Generic.Dictionary<string, string> utilityBodies,
                string pureBashFallback = null)
            {
                _functionInfo = new FunctionInfo(TypeDescriptor.Numeric, nativeName, null, ClassAccessName, false,
                    Parameters, null);
                _utilityBodies = utilityBodies;
                _pureBashFallback = pureBashFallback;
            }

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, _functionInfo, _utilityBodies,
                    functionCallStatement.Parameters, functionCallStatement.Info, _pureBashFallback);
            }
        }

        private abstract class BashBinaryNumericMathFunction<TApi> : TApi where TApi : ApiBaseFunction
        {
            private readonly FunctionInfo _functionInfo;
            private readonly System.Collections.Generic.Dictionary<string, string> _utilityBodies;
            private readonly string _pureBashFallback;

            protected BashBinaryNumericMathFunction(
                string nativeName,
                System.Collections.Generic.Dictionary<string, string> utilityBodies,
                string pureBashFallback = null)
            {
                _functionInfo = new FunctionInfo(TypeDescriptor.Numeric, nativeName, null, ClassAccessName, false,
                    Parameters, null);
                _utilityBodies = utilityBodies;
                _pureBashFallback = pureBashFallback;
            }

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, _functionInfo, _utilityBodies,
                    functionCallStatement.Parameters, functionCallStatement.Info, _pureBashFallback);
            }
        }

        public class BashMin : BashBinaryNumericMathFunction<Min>
        {
            public BashMin() : base(nameof(Min), BashMathUtilityBodies.Binary(
                "print (a < b ? a : b)",
                "if ($1<$2) $1 else $2",
                "print(min(a,b))"),
                "if [ \"$1\" -lt \"$2\" ]; then echo \"$1\"; else echo \"$2\"; fi")
            {
            }
        }

        public class BashMax : BashBinaryNumericMathFunction<Max>
        {
            public BashMax() : base(nameof(Max), BashMathUtilityBodies.Binary(
                "print (a > b ? a : b)",
                "if ($1>$2) $1 else $2",
                "print(max(a,b))"),
                "if [ \"$1\" -gt \"$2\" ]; then echo \"$1\"; else echo \"$2\"; fi")
            {
            }
        }

        public class BashFloor : BashUnaryNumericMathFunction<Floor>
        {
            public BashFloor() : base(nameof(Floor), BashMathUtilityBodies.Unary(
                "print int(a)",
                "($1)/1",
                "print(math.floor(a))"),
                "echo \"$1\" | awk '{print int($1)}'")
            {
            }
        }

        public class BashCeiling : BashUnaryNumericMathFunction<Ceiling>
        {
            public BashCeiling() : base(nameof(Ceiling), BashMathUtilityBodies.Unary(
                "print (a == int(a) ? a : int(a) + 1)",
                "($1+0.999999)/1",
                "print(math.ceil(a))"),
                "echo \"$1\" | awk '{x=$1; print (x==int(x)?x:int(x)+1)}'")
            {
            }
        }

        public class BashRound : BashUnaryNumericMathFunction<Round>
        {
            public BashRound() : base(nameof(Round), BashMathUtilityBodies.Unary(
                "print int(a + 0.5 * (a < 0 ? -1 : 1))",
                "($1+0.5)/1",
                "print(int(round(a)))"),
                "echo \"$1\" | awk '{printf \"%.0f\\n\", $1}'")
            {
            }
        }

        public class BashTruncate : BashUnaryNumericMathFunction<Truncate>
        {
            public BashTruncate() : base(nameof(Truncate), BashMathUtilityBodies.Unary(
                "print int(a)",
                "($1)/1",
                "print(int(a) if a >= 0 else -int(-a))"),
                "echo \"$1\" | awk '{print int($1)}'")
            {
            }
        }

        public class BashSqrt : BashUnaryNumericMathFunction<Sqrt>
        {
            public BashSqrt() : base(nameof(Sqrt), BashMathUtilityBodies.Unary(
                "print sqrt(a)",
                "sqrt($1)",
                "print(math.sqrt(a))"),
                "echo \"scale=10; sqrt($1)\" | bc -l 2>/dev/null || echo 0")
            {
            }
        }

        public class BashPow : BashBinaryNumericMathFunction<Pow>
        {
            public BashPow() : base(nameof(Pow), BashMathUtilityBodies.Binary(
                "print a ^ b",
                "$1 ^ $2",
                "print(a ** b)"),
                "echo \"scale=10; $1 ^ $2\" | bc -l 2>/dev/null || echo 0")
            {
            }
        }

        public class BashSign : Sign
        {
            private readonly FunctionInfo _functionInfo =
                new FunctionInfo(TypeDescriptor.Integer, nameof(Sign), null, ClassAccessName, false,
                    new FunctionParameterDefinitionStatement[] {NumberParameter}, null);

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);
                return CreateNativeMethodWithUtilityExpressionSelector(this, p, _functionInfo,
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
