using System.Collections.Generic;
using System.Globalization;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class BashMath
    {
        public class BashAbs : Abs
        {
            private const string ApiMathAbsBashMethodName = "Absolute";

            private FunctionInfo _functionInfo;

            private IDictionary<string, string> _absUtilityExpressions = new Dictionary<string, string>
            {
                {BashFunction.AwkUtilityName, "awk \"BEGIN {if ($1 < 0) { print (-($1)) } else { print ($1) }}\""},
                {BashFunction.BcUtilityName, "echo \"define abs(i) { if (i < 0) return (-i) return (i) } abs($1)\" | bc"},
            };

            public BashAbs()
            {
                _functionInfo = new FunctionInfo(TypeDescriptor.Numeric, ApiMathAbsBashMethodName,
                    null, ClassAccessName, false, Parameters, null);
            }

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(p, functionCallStatement.Parameters);

                var number = functionCallStatement.Parameters[0];

                switch (number)
                {
                    case ConstantValueStatement constantValueStatement:
                    {
                        return InlineConstant(constantValueStatement.TypeDescriptor, constantValueStatement.Value,
                            constantValueStatement);
                    }
                    case VariableAccessStatement variableAccessStatement:
                    {
                        if (p.Scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                        {
                            if (varInfo.TypeDescriptor.IsInteger())
                            {
                                return new ApiMethodBuilderRawResult(new ExpressionResult(
                                    varInfo.TypeDescriptor,
                                    $"${{{varInfo.AccessName}#-}}",
                                    variableAccessStatement
                                ));
                            }

                            return CreateNativeMethodWithUtilityExpressionSelector(this, p, _functionInfo,
                                _absUtilityExpressions, functionCallStatement.Parameters,
                                functionCallStatement.Info);
                        }

                        if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                        {
                            return InlineConstant(constInfo.TypeDescriptor, constInfo.Value, variableAccessStatement);
                        }

                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }
                    default:
                    {
                        return CreateNativeMethodWithUtilityExpressionSelector(this, p, _functionInfo,
                            _absUtilityExpressions, functionCallStatement.Parameters, functionCallStatement.Info);
                    }
                }
            }

            public static ApiMethodBuilderInlineResult InlineConstant(TypeDescriptor typeDescriptor, string value,
                IStatement statement)
            {
                if (typeDescriptor.IsNumber())
                {
                    if (typeDescriptor.IsInteger() &&
                        long.TryParse(value, out var integerResult))
                    {
                        return Inline(
                            new ConstantValueStatement(TypeDescriptor.Integer,
                                System.Math.Abs(integerResult).ToString(NumberFormatInfo.InvariantInfo),
                                statement.Info)
                        );
                    }

                    if (double.TryParse(value, out var floatResult))
                    {
                        return Inline(
                            new ConstantValueStatement(TypeDescriptor.Float,
                                System.Math.Abs(floatResult).ToString(NumberFormatInfo.InvariantInfo),
                                statement.Info)
                        );
                    }

                    throw new InvalidStatementCompilerException(statement,
                        statement.Info);
                }

                throw new TypeMismatchCompilerException(typeDescriptor, TypeDescriptor.Numeric,
                    statement.Info);
            }
        }
    }
}