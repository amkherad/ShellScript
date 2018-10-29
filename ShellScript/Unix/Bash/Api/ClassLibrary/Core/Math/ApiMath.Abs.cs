using System.Collections.Generic;
using System.Globalization;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class ApiMath
    {
        public class Abs : BashFunction
        {
            private const string ApiMathAbsBashMethodName = "Absolute";

            public override string Name => nameof(Abs);
            public override string Summary => "Returns the absolute value of the number.";
            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Numeric, "Number", null, null),
            };

            private FunctionInfo _functionInfo;

            private IDictionary<string, string> _absUtilityExpressions = new Dictionary<string, string>
            {
                {AwkUtilityName, "awk \"BEGIN {if ($1 < 0) { print (-($1)) } else { print ($1) }}\""},
                {BcUtilityName, "echo \"define abs(i) { if (i < 0) return (-i) return (i) } abs($1)\" | bc"},
            };

            public Abs()
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
                            if (varInfo.TypeDescriptor.IsDecimal())
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
                    if (typeDescriptor.IsDecimal() &&
                        long.TryParse(value, out var decimalResult))
                    {
                        return Inline(
                            new ConstantValueStatement(TypeDescriptor.Decimal,
                                System.Math.Abs(decimalResult).ToString(NumberFormatInfo.InvariantInfo),
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