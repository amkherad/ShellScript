using System;
using System.IO;
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
        public virtual bool ShouldBePinnedToFloatingPointVariable(Context context, Scope scope,
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

        public virtual bool ShouldBePinnedToFloatingPointVariable(Context context, Scope scope,
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

        public abstract string PinExpressionToVariable(Context context, Scope scope,
            TextWriter writer, DataTypes dataTypes, string nameHint, string expression, IStatement template);

        public abstract string PinFloatingPointExpressionToVariable(Context context, Scope scope,
            TextWriter writer, DataTypes dataTypes, string nameHint, string expression, IStatement template);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(Context context, Scope scope, string expression, IStatement template)
        {
            if (template is EvaluationStatement evaluationStatement)
            {
                var expressionDataType = evaluationStatement.GetDataType(context, scope);

                if (expressionDataType.IsString())
                {
                    return $"\"{expression}\"";
                }

                return expression;
            }

            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(Context context, Scope scope, DataTypes expressionDataType,
            string expression, IStatement template)
        {
            if (expressionDataType.IsString())
            {
                return $"\"{expression}\"";
            }

            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string FormatSubExpression(Context context, Scope scope, string expression, IStatement template)
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
        public virtual string FormatArithmeticExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(Context context, Scope scope, DataTypes leftDataType,
            string left, IOperator op,
            DataTypes rightDataType, string right, IStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(Context context, Scope scope, DataTypes leftDataType, string left,
            IOperator op,
            DataTypes rightDataType, string right, IStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(Context context, Scope scope, DataTypes leftDataType, string left,
            IOperator op,
            DataTypes rightDataType, string right, IStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatVariableAccessExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            return '$' + expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatFunctionCallExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            return $"$(({expression}))";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatConstantExpression(Context context, Scope scope, string expression,
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

        public virtual (DataTypes, string) CreateExpression(Context context, Scope scope,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            var (dataType, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, statement);
            return (dataType, FormatExpression(context, scope, dataType, exp, statement));
        }

        protected virtual (DataTypes, string) CreateExpressionRecursive(Context context, Scope scope,
            TextWriter nonInlinePartWriter, IStatement statement)
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
                            FormatConstantExpression(context, scope,
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
                            FormatConstantExpression(context, scope,
                                constantValueStatement.Value,
                                constantValueStatement
                            )
                        );
                    }

                    return
                    (
                        constantValueStatement.DataType,
                        FormatConstantExpression(context, scope,
                            constantValueStatement.Value,
                            constantValueStatement
                        )
                    );
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement.VariableName, out var varInfo))
                    {
                        return
                        (
                            varInfo.DataType,
                            FormatVariableAccessExpression(context, scope,
                                varInfo.AccessName,
                                variableAccessStatement
                            )
                        );
                    }

                    if (scope.TryGetConstantInfo(variableAccessStatement.VariableName, out var constInfo))
                    {
                        //should be impossible to reach.
                        return
                        (
                            constInfo.DataType,
                            FormatVariableAccessExpression(context, scope,
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
                        var (dataType, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter,
                            bitwiseEvaluationStatement.Right);

                        if (!dataType.IsDecimal())
                        {
                            throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                bitwiseEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(context, scope,
                            exp,
                            bitwiseEvaluationStatement
                        );

                        exp = $"~{exp}";

                        //can't have constant as the operand.
                        return
                        (
                            DataTypes.Decimal,
                            FormatBitwiseExpression(context, scope,
                                exp,
                                bitwiseEvaluationStatement
                            )
                        );
                    }

                    var left = bitwiseEvaluationStatement.Left;
                    var (leftDataType, leftExp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, left);

                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(left, left.Info);
                    }

                    var right = bitwiseEvaluationStatement.Right;
                    var (rightDataType, rightExp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, right);

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

                    leftExp = FormatSubExpression(context, scope,
                        leftExp,
                        left
                    );
                    rightExp = FormatSubExpression(context, scope,
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
                        FormatBitwiseExpression(context, scope,
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
                        var (dataType, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, operand);

                        if (dataType != DataTypes.Boolean)
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(context, scope,
                            exp,
                            operand
                        );

                        exp = $"!{exp}";

                        if (ShouldBePinnedToFloatingPointVariable(context, scope, dataType,
                            logicalEvaluationStatement))
                        {
                            return (dataType,
                                    PinFloatingPointExpressionToVariable(
                                        context,
                                        scope,
                                        nonInlinePartWriter,
                                        dataType,
                                        null,
                                        FormatLogicalExpression(context, scope,
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
                            FormatLogicalExpression(context, scope,
                                exp,
                                logicalEvaluationStatement
                            )
                        );
                    }

                    var left = logicalEvaluationStatement.Left;
                    var (leftDataType, leftExp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, left);

//                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
//                    {
//                        throw new InvalidStatementCompilerException(left, left.Info);
//                    }

                    var right = logicalEvaluationStatement.Right;
                    var (rightDataType, rightExp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, right);

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

                    leftExp = FormatSubExpression(context, scope,
                        leftExp,
                        left
                    );
                    rightExp = FormatSubExpression(context, scope,
                        rightExp,
                        right
                    );

                    if (ShouldBePinnedToFloatingPointVariable(context, scope, leftDataType, left, rightDataType, right))
                    {
                        return
                        (
                            DataTypes.Boolean,
                            PinFloatingPointExpressionToVariable(
                                context,
                                scope,
                                nonInlinePartWriter,
                                DataTypes.Boolean,
                                "logical",
                                FormatLogicalExpression(context, scope,
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
                        FormatLogicalExpression(context, scope,
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
                                if (scope.TryGetFunctionInfo(functionCallStatement.FunctionName, out var funcInfo))
                                {
                                    var inline = FunctionInfo.UnWrapInlinedStatement(context, scope, funcInfo);
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

                        var (dt, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, operand);

                        if (!dt.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        exp = arithmeticEvaluationStatement.Left == null
                            ? $"++{exp}"
                            : $"{exp}++";

                        if (ShouldBePinnedToFloatingPointVariable(context, scope, dt, arithmeticEvaluationStatement))
                        {
                            return
                            (
                                dt,
                                PinFloatingPointExpressionToVariable(
                                    context,
                                    scope,
                                    nonInlinePartWriter,
                                    dt,
                                    null,
                                    FormatArithmeticExpression(context, scope,
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
                            FormatArithmeticExpression(context, scope,
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
                                if (scope.TryGetFunctionInfo(functionCallStatement.FunctionName, out var funcInfo))
                                {
                                    var inline = FunctionInfo.UnWrapInlinedStatement(context, scope, funcInfo);
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

                        var (dt, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, operand);

                        if (!dt.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        exp = arithmeticEvaluationStatement.Left == null
                            ? $"--{exp}"
                            : $"{exp}--";

                        if (ShouldBePinnedToFloatingPointVariable(context, scope, dt, arithmeticEvaluationStatement))
                        {
                            return
                            (
                                dt,
                                PinFloatingPointExpressionToVariable(
                                    context,
                                    scope,
                                    nonInlinePartWriter,
                                    dt,
                                    null,
                                    FormatArithmeticExpression(context, scope,
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
                            FormatArithmeticExpression(context, scope,
                                exp,
                                arithmeticEvaluationStatement
                            )
                        );
                    }

                    if (op is NegativeNumberOperator)
                    {
                        var operand = arithmeticEvaluationStatement.Right;

                        var (dt, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, operand);

                        if (!dt.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(context, scope,
                            exp,
                            operand
                        );

                        exp = $"-{exp}";

                        if (ShouldBePinnedToFloatingPointVariable(context, scope, dt, arithmeticEvaluationStatement))
                        {
                            return
                            (
                                dt,
                                PinFloatingPointExpressionToVariable(
                                    context,
                                    scope,
                                    nonInlinePartWriter,
                                    dt,
                                    null,
                                    FormatArithmeticExpression(context, scope,
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
                            FormatArithmeticExpression(context, scope,
                                exp,
                                arithmeticEvaluationStatement
                            )
                        );
                    }

                    var left = arithmeticEvaluationStatement.Left;
                    var (leftDataType, leftExp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, left);

                    var right = arithmeticEvaluationStatement.Right;
                    var (rightDataType, rightExp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, right);

                    var dataType = StatementHelpers.OperateDataTypes(arithmeticEvaluationStatement.Operator,
                        leftDataType, rightDataType);

                    leftExp = FormatSubExpression(context, scope,
                        leftExp,
                        left
                    );
                    rightExp = FormatSubExpression(context, scope,
                        rightExp,
                        right
                    );

                    if (ShouldBePinnedToFloatingPointVariable(context, scope, leftDataType, left, rightDataType, right))
                    {
                        return
                        (
                            dataType,
                            PinFloatingPointExpressionToVariable(
                                context,
                                scope,
                                nonInlinePartWriter,
                                dataType,
                                "arithmetic",
                                FormatArithmeticExpression(context, scope,
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
                        FormatArithmeticExpression(context, scope,
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
                    if (!scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                            functionCallStatement.Info);
                    }

                    var call = new StringBuilder(20); //`myfunc 0 "test"`
                    call.Append('`');

                    call.Append(funcInfo.AccessName);

                    foreach (var param in functionCallStatement.Parameters)
                    {
                        var (dataType, exp) = CreateExpressionRecursive(context, scope, nonInlinePartWriter, param);

                        call.Append(' ');
                        call.Append(exp);
                    }

                    call.Append('`');

                    var callExp = FormatFunctionCallExpression(context, scope,
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