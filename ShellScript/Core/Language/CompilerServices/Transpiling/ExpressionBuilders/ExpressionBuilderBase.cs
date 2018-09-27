using System;
using System.Collections.Generic;
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
            DataTypes dataType, EvaluationStatement template)
        {
            return false;
        }

        public virtual bool ShouldBePinnedToFloatingPointVariable(
            ExpressionBuilderParams p, EvaluationStatement template,
            DataTypes left, EvaluationStatement leftTemplate, DataTypes right, EvaluationStatement rightTemplate)
        {
            return false;
        }

        public abstract string PinExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, EvaluationStatement template);

        public abstract string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, EvaluationStatement template);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            var expressionDataType = template.GetDataType(p.Context, p.Scope);

            if (expressionDataType.IsString())
            {
                return $"\"{expression}\"";
            }

            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(ExpressionBuilderParams p, DataTypes expressionDataType,
            string expression, EvaluationStatement template)
        {
            if (expressionDataType.IsString())
            {
                return $"\"{expression}\"";
            }

            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatSubExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
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
        public virtual string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, DataTypes leftDataType,
            string left, IOperator op,
            DataTypes rightDataType, string right, EvaluationStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left,
            IOperator op,
            DataTypes rightDataType, string right, EvaluationStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left,
            IOperator op,
            DataTypes rightDataType, string right, EvaluationStatement template)
        {
            return $"{left} {op} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatConstantExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
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

        public virtual (DataTypes, string, EvaluationStatement) CreateExpression(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            var (dataType, exp, template) = CreateExpressionRecursive(p, statement);
            return (dataType, FormatExpression(p, dataType, exp, statement), template);
        }

        protected virtual (DataTypes, string, EvaluationStatement) CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    return
                    (
                        constantValueStatement.DataType,
                        FormatConstantExpression(p,
                            constantValueStatement.IsBoolean()
                            ? constantValueStatement.Value.ToLower()
                            : constantValueStatement.Value,
                            constantValueStatement
                        ),
                        constantValueStatement
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
                            ),
                            variableAccessStatement
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
                            ),
                            variableAccessStatement
                        );
                    }

                    throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                        variableAccessStatement.Info);
                }
                case BitwiseEvaluationStatement bitwiseEvaluationStatement: //~ & |
                {
                    if (bitwiseEvaluationStatement.Operator is BitwiseNotOperator)
                    {
                        var (dataType, exp, template) = CreateExpressionRecursive(p,
                            bitwiseEvaluationStatement.Right);

                        if (!dataType.IsDecimal())
                        {
                            throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                bitwiseEvaluationStatement.Info);
                        }

                        exp = FormatSubExpression(p,
                            exp,
                            template
                        );

                        exp = $"~{exp}";

                        //can't have constant as the operand.
                        return
                        (
                            DataTypes.Decimal,
                            FormatBitwiseExpression(p,
                                exp,
                                template
                            ),
                            new BitwiseEvaluationStatement(null, bitwiseEvaluationStatement.Operator, template,
                                bitwiseEvaluationStatement.Info)
                        );
                    }

                    var left = bitwiseEvaluationStatement.Left;
                    var (leftDataType, leftExp, leftTemplate) =
                        CreateExpressionRecursive(p, left);

                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(left, left.Info);
                    }

                    var right = bitwiseEvaluationStatement.Right;
                    var (rightDataType, rightExp, rightTemplate) =
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
                        ),
                        new BitwiseEvaluationStatement(leftTemplate, bitwiseEvaluationStatement.Operator, rightTemplate,
                            bitwiseEvaluationStatement.Info)
                    );
                }
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (logicalEvaluationStatement.Operator is NotOperator)
                    {
                        var operand = logicalEvaluationStatement.Right;
                        var (dataType, exp, template) = CreateExpressionRecursive(p, operand);

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
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                dataType,
                                null,
                                FormatLogicalExpression(p,
                                    exp,
                                    logicalEvaluationStatement
                                ),
                                logicalEvaluationStatement
                            );
                            return (
                                dataType,
                                varName,
                                new VariableAccessStatement(varName, logicalEvaluationStatement.Info)
                            );
                        }

                        return
                        (
                            DataTypes.Boolean,
                            FormatLogicalExpression(p,
                                exp,
                                logicalEvaluationStatement
                            ),
                            new LogicalEvaluationStatement(null, logicalEvaluationStatement.Operator, template,
                                logicalEvaluationStatement.Info)
                        );
                    }

                    var left = logicalEvaluationStatement.Left;
                    var (leftDataType, leftExp, leftTemplate) = CreateExpressionRecursive(p, left);

//                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
//                    {
//                        throw new InvalidStatementCompilerException(left, left.Info);
//                    }

                    var right = logicalEvaluationStatement.Right;
                    var (rightDataType, rightExp, rightTemplate) =
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

                    if (ShouldBePinnedToFloatingPointVariable(p, logicalEvaluationStatement, leftDataType, left,
                        rightDataType, right))
                    {
                        var varName = PinFloatingPointExpressionToVariable(
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
                        );

                        return
                        (
                            DataTypes.Boolean,
                            varName,
                            new VariableAccessStatement(varName, logicalEvaluationStatement.Info)
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
                        ),
                        new LogicalEvaluationStatement(leftTemplate, logicalEvaluationStatement.Operator,
                            rightTemplate, logicalEvaluationStatement.Info)
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

                        var (dt, exp, template) = CreateExpressionRecursive(p, operand);

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
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                dt,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );

                            return
                            (
                                dt,
                                varName,
                                new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                            );
                        }

                        return
                        (
                            dt,
                            FormatArithmeticExpression(p,
                                exp,
                                arithmeticEvaluationStatement
                            ),
                            arithmeticEvaluationStatement.Left == null
                                ? new ArithmeticEvaluationStatement(null, arithmeticEvaluationStatement.Operator,
                                    template, arithmeticEvaluationStatement.Info)
                                : new ArithmeticEvaluationStatement(template, arithmeticEvaluationStatement.Operator,
                                    null, arithmeticEvaluationStatement.Info)
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

                        var (dt, exp, template) = CreateExpressionRecursive(p, operand);

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
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                dt,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );

                            return
                            (
                                dt,
                                varName,
                                new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                            );
                        }

                        return
                        (
                            dt,
                            FormatArithmeticExpression(p,
                                exp,
                                arithmeticEvaluationStatement
                            ),
                            arithmeticEvaluationStatement.Left == null
                                ? new ArithmeticEvaluationStatement(null, arithmeticEvaluationStatement.Operator,
                                    template, arithmeticEvaluationStatement.Info)
                                : new ArithmeticEvaluationStatement(template, arithmeticEvaluationStatement.Operator,
                                    null, arithmeticEvaluationStatement.Info)
                        );
                    }

                    if (op is NegativeNumberOperator)
                    {
                        var operand = arithmeticEvaluationStatement.Right;

                        var (dt, exp, template) = CreateExpressionRecursive(p, operand);

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
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                dt,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );

                            return
                            (
                                dt,
                                varName,
                                new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                            );
                        }

                        return
                        (
                            dt,
                            FormatArithmeticExpression(p,
                                exp,
                                arithmeticEvaluationStatement
                            ),
                            new ArithmeticEvaluationStatement(null, arithmeticEvaluationStatement.Operator,
                                template, arithmeticEvaluationStatement.Info)
                        );
                    }

                    var left = arithmeticEvaluationStatement.Left;
                    var (leftDataType, leftExp, leftTemplate) = CreateExpressionRecursive(p, left);

                    var right = arithmeticEvaluationStatement.Right;
                    var (rightDataType, rightExp, rightTemplate) =
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

                    if (ShouldBePinnedToFloatingPointVariable(p, arithmeticEvaluationStatement, leftDataType, left,
                        rightDataType, right))
                    {
                        var varName = PinFloatingPointExpressionToVariable(
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
                        );

                        return
                        (
                            dataType,
                            varName,
                            new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                        );
                    }

                    var newTmp = new ArithmeticEvaluationStatement(leftTemplate, arithmeticEvaluationStatement.Operator,
                        rightTemplate, arithmeticEvaluationStatement.Info);

                    leftTemplate.ParentStatement = newTmp;
                    rightTemplate.ParentStatement = newTmp;
                    
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
                        ),
                        newTmp
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
                                return (rawResult.DataType, rawResult.Expression, rawResult.Template);
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

                    var paramTemplates = new List<EvaluationStatement>();
                    foreach (var param in functionCallStatement.Parameters)
                    {
                        var (dataType, exp, template) =
                            CreateExpressionRecursive(p, param);

                        call.Append(' ');
                        call.Append(FormatFunctionCallParameterSubExpression(p, dataType, exp, template));

                        paramTemplates.Add(template);
                    }

                    call.Append('`');

                    var callExp = FormatFunctionCallExpression(
                        p,
                        call.ToString(),
                        functionCallStatement
                    );

                    var paramsArray = paramTemplates.ToArray();
                    var newTmp = new FunctionCallStatement(functionCallStatement.ObjectName,
                        functionCallStatement.FunctionName,
                        functionCallStatement.DataType, paramsArray, functionCallStatement.Info);
                    
                    foreach (var param in paramsArray)
                    {
                        param.ParentStatement = newTmp;
                    }

                    return
                    (
                        functionCallStatement.DataType,
                        callExp,
                        newTmp
                    );
                }

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}