using System;
using System.Runtime.CompilerServices;
using System.Text;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public abstract class ExpressionBuilderBase : IExpressionBuilder
    {
        public virtual bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes dataType, IStatement template)
        {
            return false;

//            if (template is ConstantValueStatement)
//                return false;
//            if (template is VariableAccessStatement)
//                return false;
//            if (template is FunctionCallStatement)
//                return false;
//
//            return dataType.IsNumericOrFloat();
        }

        public virtual bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes left, IStatement leftTemplate, DataTypes right, IStatement rightTemplate)
        {
            return false;

//            if (leftTemplate is ConstantValueStatement)
//                return false;
//            if (leftTemplate is VariableAccessStatement)
//                return false;
//            if (leftTemplate is FunctionCallStatement)
//                return false;
//
//            return left.IsNumericOrFloat();
        }

        public abstract string PinExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, IStatement template);

        public abstract string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, IStatement template);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(ExpressionBuilderParams p, string expression, IStatement template)
        {
            if (template is EvaluationStatement evaluationStatement)
            {
                var expressionDataType = evaluationStatement.GetDataType(p.Context, p.Scope);

                if (expressionDataType.IsString())
                {
                    return $"\"{expression}\"";
                }

                return expression;
            }

            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(ExpressionBuilderParams p, DataTypes expressionDataType,
            string expression, IStatement template)
        {
            if (expressionDataType.IsString())
            {
                return $"\"{expression}\"";
            }

            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatSubExpression(ExpressionBuilderParams p, string expression, IStatement template)
        {
            if (template is ConstantValueStatement)
                return expression;
            if (template is VariableAccessStatement)
                return expression;
            if (template is FunctionCallStatement)
                return expression;

            return $"({expression})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, DataTypes leftDataType,
            string left, IOperator op,
            DataTypes rightDataType, string right, IStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left,
            IOperator op,
            DataTypes rightDataType, string right, IStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left,
            IOperator op,
            DataTypes rightDataType, string right, IStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return '$' + expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatConstantExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    var value = constantValueStatement.Value;
                    if (value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        return value;
                    }

                    return $"\"{value}\"";
                }
            }

            return expression;
        }

        public virtual (DataTypes, string) CreateExpression(ExpressionBuilderParams p, IStatement statement)
        {
            var (dataType, exp) = CreateExpressionRecursive(p, statement);
            return (dataType, FormatExpression(p, dataType, exp, statement));
        }

        protected virtual (DataTypes, string) CreateExpressionRecursive(ExpressionBuilderParams p, IStatement statement)
        {
            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    if (constantValueStatement.IsNumber())
                    {
                        return
                        (
                            constantValueStatement.DataType,
                            FormatConstantExpression(p,
                                constantValueStatement.Value,
                                constantValueStatement
                            )
                        );
                    }

                    if (constantValueStatement.IsString())
                    {
                        return
                        (
                            DataTypes.String,
                            FormatConstantExpression(p,
                                constantValueStatement.Value,
                                constantValueStatement
                            )
                        );
                    }

                    return
                    (
                        constantValueStatement.DataType,
                        FormatConstantExpression(p,
                            constantValueStatement.Value,
                            constantValueStatement
                        )
                    );
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (p.Scope.TryGetVariableInfo(variableAccessStatement.VariableName, out var varInfo))
                    {
                        return
                        (
                            varInfo.DataType,
                            FormatVariableAccessExpression(p,
                                varInfo.AccessName,
                                variableAccessStatement
                            )
                        );
                    }

                    if (p.Scope.TryGetConstantInfo(variableAccessStatement.VariableName, out var constInfo))
                    {
                        //should be impossible to reach.
                        return
                        (
                            constInfo.DataType,
                            FormatVariableAccessExpression(p,
                                constInfo.AccessName,
                                variableAccessStatement
                            )
                        );
                    }

                    throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                        variableAccessStatement.Info);
                }
                case BitwiseEvaluationStatement bitwiseEvaluationStatement: //~ & |
                {
                    if (bitwiseEvaluationStatement.Operator is BitwiseNotOperator)
                    {
                        var (dataType, exp) = CreateExpressionRecursive(p,
                            bitwiseEvaluationStatement.Right);

                        if (!dataType.IsDecimal())
                        {
                            throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                bitwiseEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(p,
                            exp,
                            bitwiseEvaluationStatement
                        );

                        exp = $"~{exp}";

                        //can't have constant as the operand.
                        return
                        (
                            DataTypes.Decimal,
                            FormatBitwiseExpression(p,
                                exp,
                                bitwiseEvaluationStatement
                            )
                        );
                    }

                    var left = bitwiseEvaluationStatement.Left;
                    var (leftDataType, leftExp) =
                        CreateExpressionRecursive(p, left);

                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(left, left.Info);
                    }

                    var right = bitwiseEvaluationStatement.Right;
                    var (rightDataType, rightExp) =
                        CreateExpressionRecursive(p, right);

                    if (!(rightDataType.IsDecimal() || rightDataType.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                            bitwiseEvaluationStatement.Info);
                    }

                    if (leftDataType != rightDataType)
                    {
                        throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                            bitwiseEvaluationStatement.Info);
                    }

                    leftExp = FormatSubExpression(p,
                        leftExp,
                        left
                    );
                    rightExp = FormatSubExpression(p,
                        rightExp,
                        right
                    );

//                    if (leftDataType.IsNumericOrFloat() || rightDataType.IsNumericOrFloat())
//                    {
//                        return (leftDataType,
//                                PinFloatingPointExpressionToVariable(
//                                    context,
//                                    scope,
//                                    nonInlinePartWriter,
//                                    leftDataType,
//                                    "bitwise",
//                                    FormatBitwiseExpression(
//                                        leftDataType,
//                                        leftExp,
//                                        bitwiseEvaluationStatement.Operator,
//                                        rightDataType,
//                                        rightExp,
//                                        bitwiseEvaluationStatement
//                                    ),
//                                    bitwiseEvaluationStatement
//                                )
//                            );
//                    }

                    return
                    (
                        leftDataType,
                        FormatBitwiseExpression(p,
                            leftDataType,
                            leftExp,
                            bitwiseEvaluationStatement.Operator,
                            rightDataType,
                            rightExp,
                            bitwiseEvaluationStatement
                        )
                    );
                }
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (logicalEvaluationStatement.Operator is NotOperator)
                    {
                        var operand = logicalEvaluationStatement.Right;
                        var (dataType, exp) = CreateExpressionRecursive(p, operand);

                        if (dataType != DataTypes.Boolean)
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(p,
                            exp,
                            operand
                        );

                        exp = $"! {exp}";

                        if (ShouldBePinnedToFloatingPointVariable(p, dataType,
                            logicalEvaluationStatement))
                        {
                            return (dataType,
                                    PinFloatingPointExpressionToVariable(
                                        p,
                                        dataType,
                                        null,
                                        FormatLogicalExpression(p,
                                            exp,
                                            logicalEvaluationStatement
                                        ),
                                        logicalEvaluationStatement
                                    )
                                );
                        }

                        return
                        (
                            DataTypes.Boolean,
                            FormatLogicalExpression(p,
                                exp,
                                logicalEvaluationStatement
                            )
                        );
                    }

                    var left = logicalEvaluationStatement.Left;
                    var (leftDataType, leftExp) = CreateExpressionRecursive(p, left);

//                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
//                    {
//                        throw new InvalidStatementCompilerException(left, left.Info);
//                    }

                    var right = logicalEvaluationStatement.Right;
                    var (rightDataType, rightExp) =
                        CreateExpressionRecursive(p, right);

//                    if (!(rightDataType.IsDecimal() || rightDataType.IsBoolean()))
//                    {
//                        throw new InvalidStatementCompilerException(logicalEvaluationStatement,
//                            logicalEvaluationStatement.Info);
//                    }

                    if (leftDataType != rightDataType)
                    {
                        if (!(leftDataType.IsNumber() && rightDataType.IsNumber()))
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }
                    }

                    leftExp = FormatSubExpression(p,
                        leftExp,
                        left
                    );
                    rightExp = FormatSubExpression(p,
                        rightExp,
                        right
                    );

                    if (ShouldBePinnedToFloatingPointVariable(p, leftDataType, left, rightDataType, right))
                    {
                        return
                        (
                            DataTypes.Boolean,
                            PinFloatingPointExpressionToVariable(
                                p,
                                DataTypes.Boolean,
                                "logical",
                                FormatLogicalExpression(p,
                                    leftDataType,
                                    leftExp,
                                    logicalEvaluationStatement.Operator,
                                    rightDataType,
                                    rightExp,
                                    logicalEvaluationStatement
                                ),
                                logicalEvaluationStatement
                            )
                        );
                    }

                    return
                    (
                        DataTypes.Boolean,
                        FormatLogicalExpression(p,
                            leftDataType,
                            leftExp,
                            logicalEvaluationStatement.Operator,
                            rightDataType,
                            rightExp,
                            logicalEvaluationStatement
                        )
                    );
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    var op = arithmeticEvaluationStatement.Operator;
                    if (op is IncrementOperator)
                    {
                        var operand = arithmeticEvaluationStatement.Left ?? arithmeticEvaluationStatement.Right;
                        if (!(operand is VariableAccessStatement))
                        {
                            var isError = true;
                            if (operand is FunctionCallStatement functionCallStatement)
                            {
                                if (p.Scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
                                {
                                    var inline = FunctionInfo.UnWrapInlinedStatement(p.Context, p.Scope, funcInfo);
                                    if (inline is EvaluationStatement evalStatement)
                                    {
                                        operand = evalStatement;
                                        isError = false;
                                    }
                                }
                            }

                            if (isError)
                            {
                                throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                    arithmeticEvaluationStatement.Info);
                            }
                        }

                        var (dt, exp) = CreateExpressionRecursive(p, operand);

                        if (!dt.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        exp = arithmeticEvaluationStatement.Left == null
                            ? $"++{exp}"
                            : $"{exp}++";

                        if (ShouldBePinnedToFloatingPointVariable(p, dt, arithmeticEvaluationStatement))
                        {
                            return
                            (
                                dt,
                                PinFloatingPointExpressionToVariable(
                                    p,
                                    dt,
                                    null,
                                    FormatArithmeticExpression(p,
                                        exp,
                                        arithmeticEvaluationStatement
                                    ),
                                    arithmeticEvaluationStatement
                                )
                            );
                        }

                        return
                        (
                            dt,
                            FormatArithmeticExpression(p,
                                exp,
                                arithmeticEvaluationStatement
                            )
                        );
                    }

                    if (op is DecrementOperator)
                    {
                        var operand = arithmeticEvaluationStatement.Left ?? arithmeticEvaluationStatement.Right;
                        if (!(operand is VariableAccessStatement))
                        {
                            var isError = true;
                            if (operand is FunctionCallStatement functionCallStatement)
                            {
                                if (p.Scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
                                {
                                    var inline = FunctionInfo.UnWrapInlinedStatement(p.Context, p.Scope, funcInfo);
                                    if (inline is EvaluationStatement evalStatement)
                                    {
                                        operand = evalStatement;
                                        isError = false;
                                    }
                                }
                            }

                            if (isError)
                            {
                                throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                    arithmeticEvaluationStatement.Info);
                            }
                        }

                        var (dt, exp) = CreateExpressionRecursive(p, operand);

                        if (!dt.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        exp = arithmeticEvaluationStatement.Left == null
                            ? $"--{exp}"
                            : $"{exp}--";

                        if (ShouldBePinnedToFloatingPointVariable(p, dt, arithmeticEvaluationStatement))
                        {
                            return
                            (
                                dt,
                                PinFloatingPointExpressionToVariable(
                                    p,
                                    dt,
                                    null,
                                    FormatArithmeticExpression(p,
                                        exp,
                                        arithmeticEvaluationStatement
                                    ),
                                    arithmeticEvaluationStatement
                                )
                            );
                        }

                        return
                        (
                            dt,
                            FormatArithmeticExpression(p,
                                exp,
                                arithmeticEvaluationStatement
                            )
                        );
                    }

                    if (op is NegativeNumberOperator)
                    {
                        var operand = arithmeticEvaluationStatement.Right;

                        var (dt, exp) = CreateExpressionRecursive(p, operand);

                        if (!dt.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(p,
                            exp,
                            operand
                        );

                        exp = $"-{exp}";

                        if (ShouldBePinnedToFloatingPointVariable(p, dt, arithmeticEvaluationStatement))
                        {
                            return
                            (
                                dt,
                                PinFloatingPointExpressionToVariable(
                                    p,
                                    dt,
                                    null,
                                    FormatArithmeticExpression(p,
                                        exp,
                                        arithmeticEvaluationStatement
                                    ),
                                    arithmeticEvaluationStatement
                                )
                            );
                        }

                        return
                        (
                            dt,
                            FormatArithmeticExpression(p,
                                exp,
                                arithmeticEvaluationStatement
                            )
                        );
                    }

                    var left = arithmeticEvaluationStatement.Left;
                    var (leftDataType, leftExp) = CreateExpressionRecursive(p, left);

                    var right = arithmeticEvaluationStatement.Right;
                    var (rightDataType, rightExp) =
                        CreateExpressionRecursive(p, right);

                    var dataType = StatementHelpers.OperateDataTypes(arithmeticEvaluationStatement.Operator,
                        leftDataType, rightDataType);

                    leftExp = FormatSubExpression(p,
                        leftExp,
                        left
                    );
                    rightExp = FormatSubExpression(p,
                        rightExp,
                        right
                    );

                    if (ShouldBePinnedToFloatingPointVariable(p, leftDataType, left, rightDataType, right))
                    {
                        return
                        (
                            dataType,
                            PinFloatingPointExpressionToVariable(
                                p,
                                dataType,
                                "arithmetic",
                                FormatArithmeticExpression(p,
                                    leftDataType,
                                    leftExp,
                                    arithmeticEvaluationStatement.Operator,
                                    rightDataType,
                                    rightExp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            )
                        );
                    }

                    return
                    (
                        dataType,
                        FormatArithmeticExpression(p,
                            leftDataType,
                            leftExp,
                            arithmeticEvaluationStatement.Operator,
                            rightDataType,
                            rightExp,
                            arithmeticEvaluationStatement
                        )
                    );
                }
                case FunctionCallStatement functionCallStatement: //functions are always not-inlined.
                {
                    if (!p.Scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                            functionCallStatement.Info);
                    }

                    if (funcInfo is ApiFunctionInfo apiFunctionInfo)
                    {
                        var result = apiFunctionInfo.Function.Build(p, functionCallStatement);

                        switch (result)
                        {
                            case ApiMethodBuilderRawResult rawResult:
                            {
                                return (rawResult.DataType, rawResult.Expression);
                            }
                            case ApiMethodBuilderInlineResult inlineResult:
                            {
                                return CreateExpression(p, inlineResult.Statement);
                            }

                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    var call = new StringBuilder(20); //`myfunc 0 "test"`
                    call.Append('`');

                    call.Append(funcInfo.AccessName);

                    foreach (var param in functionCallStatement.Parameters)
                    {
                        var (dataType, exp) =
                            CreateExpressionRecursive(p, param);

                        call.Append(' ');
                        call.Append(exp);
                    }

                    call.Append('`');

                    var callExp = FormatFunctionCallExpression(
                        p,
                        call.ToString(),
                        functionCallStatement
                    );

                    return
                    (
                        functionCallStatement.DataType,
                        callExp
                    );
                }

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}