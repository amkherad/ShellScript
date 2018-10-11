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
            private const string ApiMathAbsBashMethodName = "Abs_Bash";

            public override string Name => nameof(Abs);
            public override string Summary => "Returns the absolute value of the number.";
            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Numeric;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(DataTypes.Numeric, "Number", null, null),
            };

            private FunctionInfo _absFunctionInfo;

            private IDictionary<string, string> _absUtilityExpressions = new Dictionary<string, string>
            {
                {AwkUtilityName, "awk \"BEGIN {if ($1 < 0) { print (-($1)) } else { print ($1) }}\""},
                {BcUtilityName, "echo \"define abs(i) { if (i < 0) return (-i) return (i) } abs($1)\" | bc"},
            };

            public Abs()
            {
                _absFunctionInfo = new FunctionInfo(DataTypes.Numeric, ApiMathAbsBashMethodName,
                    null, ClassAccessName, false, Parameters, null);
            }

            public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                AssertParameters(functionCallStatement.Parameters);

                var number = functionCallStatement.Parameters[0];

                switch (number)
                {
                    case ConstantValueStatement constantValueStatement:
                    {
                        return InlineConstant(constantValueStatement.DataType, constantValueStatement.Value,
                            constantValueStatement);
                    }
                    case VariableAccessStatement variableAccessStatement:
                    {
                        if (p.Scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                        {
                            if (varInfo.DataType.IsDecimal())
                            {
                                return new ApiMethodBuilderRawResult(new ExpressionResult(
                                    varInfo.DataType,
                                    $"${{{varInfo.AccessName}#-}}",
                                    variableAccessStatement
                                ));
                            }

                            return CreateNativeMethodWithUtilityExpressionSelector(this, p, _absFunctionInfo,
                                _absUtilityExpressions, functionCallStatement.Parameters,
                                functionCallStatement.Info);
                        }

                        if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                        {
                            return InlineConstant(constInfo.DataType, constInfo.Value, variableAccessStatement);
                        }

                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }
                    default:
                    {
                        return CreateNativeMethodWithUtilityExpressionSelector(this, p, _absFunctionInfo,
                            _absUtilityExpressions, functionCallStatement.Parameters, functionCallStatement.Info);
                    }
                }
            }

            public static ApiMethodBuilderInlineResult InlineConstant(DataTypes dataType, string value,
                IStatement statement)
            {
                if (dataType.IsNumber())
                {
                    if (dataType.IsDecimal() &&
                        long.TryParse(value, out var decimalResult))
                    {
                        return Inline(
                            new ConstantValueStatement(DataTypes.Decimal,
                                System.Math.Abs(decimalResult).ToString(NumberFormatInfo.InvariantInfo),
                                statement.Info)
                        );
                    }

                    if (double.TryParse(value, out var floatResult))
                    {
                        return Inline(
                            new ConstantValueStatement(DataTypes.Float,
                                System.Math.Abs(floatResult).ToString(NumberFormatInfo.InvariantInfo),
                                statement.Info)
                        );
                    }

                    throw new InvalidStatementCompilerException(statement,
                        statement.Info);
                }

                throw new TypeMismatchCompilerException(dataType, DataTypes.Numeric,
                    statement.Info);
            }
        }
    }
}