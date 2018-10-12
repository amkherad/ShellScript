using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public abstract class ExpressionBuilderBase : IExpressionBuilder
    {
        public static readonly IExpressionNotice[] PinRequiredNotice =
        {
            new PinRequiredNotice(),
        };

        public virtual bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes dataType, EvaluationStatement template)
        {
            return false;
        }

        public virtual bool ShouldBePinnedToFloatingPointVariable(
            ExpressionBuilderParams p, EvaluationStatement template, ExpressionResult left, ExpressionResult right)
        {
            return false;
        }

        public virtual bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            EvaluationStatement template, DataTypes left,
            EvaluationStatement leftTemplate, DataTypes right, EvaluationStatement rightTemplate)
        {
            return false;
        }

        public abstract string PinExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result);

        public abstract string PinExpressionToVariable(ExpressionBuilderParams p, DataTypes dataTypes, string nameHint,
            string expression, EvaluationStatement template);


        public abstract string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result);

        public abstract string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p, DataTypes dataTypes,
            string nameHint, string expression, EvaluationStatement template);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.DataType.IsString())
            {
                if (p.FormatString)
                    return $"\"{result.Expression}\"";
                return result.Expression;
            }

            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatSubExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement)
                return result.Expression;
            if (result.Template is VariableAccessStatement)
                return result.Expression;
            if (result.Template is FunctionCallStatement)
                return result.Expression;

            return $"({result.Expression})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p,
            ExpressionResult result)
        {
            return result.Expression;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, DataTypes leftDataType,
            string leftExp,
            IOperator op,
            DataTypes rightDataType, string rightExp, EvaluationStatement template)
        {
            return $"{leftExp} {op} {rightExp}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template)
        {
            return $"{left.Expression} {op} {right.Expression}";
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, DataTypes leftDataType, string leftExp,
            IOperator op, DataTypes rightDataType, string rightExp, EvaluationStatement template)
        {
            return $"{leftExp} {op} {rightExp}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, ExpressionResult left,
            IOperator op, ExpressionResult right, EvaluationStatement template)
        {
            return $"{left.Expression} {op} {right.Expression}";
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, DataTypes leftDataType, string leftExp,
            IOperator op, DataTypes rightDataType, string rightExp, EvaluationStatement template)
        {
            return $"{leftExp} {op} {rightExp}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult left,
            IOperator op, ExpressionResult right, EvaluationStatement template)
        {
            return $"{left.Expression} {op} {right.Expression}";
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatVariableAccessExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatFunctionCallExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatFunctionCallExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    var value = constantValueStatement.Value;
                    if (p.FormatString)
                    {
                        value = BashTranspilerHelpers.ToBashString(value, true, false);
                    }

                    if (value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        return value;
                    }

                    return $"\"{value}\"";
                }
            }

            return result.Expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatConstantExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    var value = constantValueStatement.Value;
                    if (p.FormatString)
                    {
                        value = BashTranspilerHelpers.ToBashString(value, true, false);
                    }
                    
                    if (value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        return value;
                    }

                    return $"\"{value}\"";
                }
            }

            return expression;
        }

        public virtual ExpressionResult CreateExpression(ExpressionBuilderParams p, EvaluationStatement statement)
        {
            var result = CreateExpressionRecursive(p, statement);
            return new ExpressionResult(
                result.DataType,
                FormatExpression(p, result),
                result.Template
            );
        }

        protected virtual void HandleNotices(ExpressionBuilderParams p, ref ExpressionResult left,
            ref ExpressionResult right)
        {
            if (right.Notices != null && right.Notices.Length > 0)
            {
                foreach (var notice in right.Notices)
                {
                    switch (notice)
                    {
                        case PinRequiredNotice _:
                        {
                            switch (left.Template)
                            {
                                case ConstantValueStatement _:
                                case VariableAccessStatement _:
                                    continue;
                            }

                            var varName = PinExpressionToVariable(p, null, left);

                            var template = new VariableAccessStatement(varName, left.Template.Info);

                            left = new ExpressionResult(left.DataType, varName, template);

                            template.ParentStatement = left.Template.ParentStatement;

                            break;
                        }
                    }
                }

                //right.Notices = null;
            }
        }

        protected virtual ExpressionResult CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    return new ExpressionResult(
                        constantValueStatement.DataType,
                        FormatConstantExpression(p,
                            constantValueStatement.DataType,
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
                    if (p.Scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                    {
                        return new ExpressionResult(
                            varInfo.DataType,
                            FormatVariableAccessExpression(p,
                                varInfo.DataType,
                                varInfo.AccessName,
                                variableAccessStatement
                            ),
                            variableAccessStatement
                        );
                    }

                    if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                    {
                        //should be impossible to reach.
                        return new ExpressionResult(
                            constInfo.DataType,
                            FormatVariableAccessExpression(p,
                                constInfo.DataType,
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
                        var result = CreateExpressionRecursive(p, bitwiseEvaluationStatement.Right);

                        if (!result.DataType.IsDecimal())
                        {
                            throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                                bitwiseEvaluationStatement.Info);
                        }

                        var exp = FormatSubExpression(p, result);

                        exp = $"~{exp}";

                        //can't have constant as the operand.
                        {
                            var newTemp = new BitwiseEvaluationStatement(null, bitwiseEvaluationStatement.Operator,
                                result.Template, bitwiseEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                DataTypes.Decimal,
                                FormatBitwiseExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
                    }

                    var leftResult = CreateExpressionRecursive(p, bitwiseEvaluationStatement.Left);

                    if (!(leftResult.DataType.IsDecimal() || leftResult.DataType.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(leftResult.Template, leftResult.Template.Info);
                    }

                    var rightResult = CreateExpressionRecursive(p, bitwiseEvaluationStatement.Right);

                    if (!(rightResult.DataType.IsDecimal() || rightResult.DataType.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                            bitwiseEvaluationStatement.Info);
                    }

                    HandleNotices(p, ref leftResult, ref rightResult);

                    if (leftResult.DataType != rightResult.DataType)
                    {
                        throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                            bitwiseEvaluationStatement.Info);
                    }

                    var leftExp = FormatSubExpression(p, leftResult);
                    var rightExp = FormatSubExpression(p, rightResult);

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

                    {
                        var newTemp = new BitwiseEvaluationStatement(leftResult.Template,
                            bitwiseEvaluationStatement.Operator,
                            rightResult.Template, bitwiseEvaluationStatement.Info);

                        leftResult.Template.ParentStatement = newTemp;
                        rightResult.Template.ParentStatement = newTemp;

                        return new ExpressionResult(
                            leftResult.DataType,
                            FormatBitwiseExpression(p,
                                leftResult.DataType,
                                leftExp,
                                bitwiseEvaluationStatement.Operator,
                                rightResult.DataType,
                                rightExp,
                                newTemp
                            ),
                            newTemp
                        );
                    }
                }
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (logicalEvaluationStatement.Operator is NotOperator)
                    {
                        var result = CreateExpressionRecursive(p, logicalEvaluationStatement.Right);

                        if (result.DataType != DataTypes.Boolean)
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }

                        var exp = FormatSubExpression(p, result);

                        exp = $"! {exp}";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.DataType,
                            logicalEvaluationStatement))
                        {
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                result.DataType,
                                null,
                                FormatLogicalExpression(p,
                                    exp,
                                    logicalEvaluationStatement
                                ),
                                logicalEvaluationStatement
                            );
                            return new ExpressionResult(
                                result.DataType,
                                varName,
                                new VariableAccessStatement(varName, logicalEvaluationStatement.Info)
                            );
                        }

                        {
                            var newTemp = new LogicalEvaluationStatement(null, logicalEvaluationStatement.Operator,
                                result.Template, logicalEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                DataTypes.Boolean,
                                FormatLogicalExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
                    }

                    var leftResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Left);

//                    if (!(leftDataType.IsDecimal() || leftDataType.IsBoolean()))
//                    {
//                        throw new InvalidStatementCompilerException(left, left.Info);
//                    }

                    var rightResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Right);

//                    if (!(rightDataType.IsDecimal() || rightDataType.IsBoolean()))
//                    {
//                        throw new InvalidStatementCompilerException(logicalEvaluationStatement,
//                            logicalEvaluationStatement.Info);
//                    }

                    HandleNotices(p, ref leftResult, ref rightResult);

                    if (leftResult.DataType != rightResult.DataType)
                    {
                        //TODO: fix if you want to support string comparison or etc.
                        if (!(leftResult.DataType.IsNumber() && rightResult.DataType.IsNumber()))
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }
                    }

                    var leftExp = FormatSubExpression(p, leftResult);
                    var rightExp = FormatSubExpression(p, rightResult);

                    if (ShouldBePinnedToFloatingPointVariable(p, logicalEvaluationStatement, leftResult, rightResult))
                    {
                        var varName = PinFloatingPointExpressionToVariable(
                            p,
                            DataTypes.Boolean,
                            "logical",
                            FormatLogicalExpression(p,
                                leftResult,
                                logicalEvaluationStatement.Operator,
                                rightResult,
                                logicalEvaluationStatement
                            ),
                            logicalEvaluationStatement
                        );

                        return new ExpressionResult(
                            DataTypes.Boolean,
                            varName,
                            new VariableAccessStatement(varName, logicalEvaluationStatement.Info)
                        );
                    }

                    {
                        var newTemp = new LogicalEvaluationStatement(leftResult.Template,
                            logicalEvaluationStatement.Operator,
                            rightResult.Template, logicalEvaluationStatement.Info);

                        leftResult.Template.ParentStatement = newTemp;
                        rightResult.Template.ParentStatement = newTemp;


                        return new ExpressionResult(
                            DataTypes.Boolean,
                            FormatLogicalExpression(p,
                                leftResult.DataType,
                                leftExp,
                                logicalEvaluationStatement.Operator,
                                rightResult.DataType,
                                rightExp,
                                newTemp
                            ),
                            newTemp
                        );
                    }
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

                        var result = CreateExpressionRecursive(p, operand);

                        if (!result.DataType.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var exp = arithmeticEvaluationStatement.Left == null
                            ? $"++{result.Expression}"
                            : $"{result.Expression}++";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.DataType, arithmeticEvaluationStatement))
                        {
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                result.DataType,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );

                            return new ExpressionResult(
                                result.DataType,
                                varName,
                                new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                            );
                        }

                        {
                            var newTemp = arithmeticEvaluationStatement.Left == null
                                ? new ArithmeticEvaluationStatement(null, arithmeticEvaluationStatement.Operator,
                                    result.Template, arithmeticEvaluationStatement.Info)
                                : new ArithmeticEvaluationStatement(result.Template,
                                    arithmeticEvaluationStatement.Operator,
                                    null, arithmeticEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                result.DataType,
                                FormatArithmeticExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
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

                        var result = CreateExpressionRecursive(p, operand);

                        if (!result.DataType.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var exp = arithmeticEvaluationStatement.Left == null
                            ? $"--{result.Expression}"
                            : $"{result.Expression}--";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.DataType, arithmeticEvaluationStatement))
                        {
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                result.DataType,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );

                            return new ExpressionResult(
                                result.DataType,
                                varName,
                                new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                            );
                        }

                        {
                            var newTemp = arithmeticEvaluationStatement.Left == null
                                ? new ArithmeticEvaluationStatement(null, arithmeticEvaluationStatement.Operator,
                                    result.Template, arithmeticEvaluationStatement.Info)
                                : new ArithmeticEvaluationStatement(result.Template,
                                    arithmeticEvaluationStatement.Operator,
                                    null, arithmeticEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                result.DataType,
                                FormatArithmeticExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
                    }

                    if (op is NegativeNumberOperator)
                    {
                        var operand = arithmeticEvaluationStatement.Right;

                        var result = CreateExpressionRecursive(p, operand);

                        if (!result.DataType.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var exp = FormatSubExpression(p, result);

                        exp = $"-{exp}";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.DataType, arithmeticEvaluationStatement))
                        {
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                result.DataType,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );

                            return new ExpressionResult(
                                result.DataType,
                                varName,
                                new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                            );
                        }

                        {
                            var newTemp = new ArithmeticEvaluationStatement(null,
                                arithmeticEvaluationStatement.Operator, result.Template,
                                arithmeticEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                result.DataType,
                                FormatArithmeticExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
                    }

                    var leftResult = CreateExpressionRecursive(p, arithmeticEvaluationStatement.Left);
                    var rightResult = CreateExpressionRecursive(p, arithmeticEvaluationStatement.Right);

                    HandleNotices(p, ref leftResult, ref rightResult);

                    if (leftResult.DataType.IsString() || rightResult.DataType.IsString() &&
                        !(arithmeticEvaluationStatement.Operator is AdditionOperator))
                    {
                        throw new InvalidOperatorForTypeCompilerException(
                            arithmeticEvaluationStatement.Operator.GetType(), leftResult.DataType, rightResult.DataType,
                            arithmeticEvaluationStatement.Info);
                    }

                    var dataType = StatementHelpers.OperateDataTypes(arithmeticEvaluationStatement.Operator,
                        leftResult.DataType, rightResult.DataType);

                    var leftExp = FormatSubExpression(p, leftResult);
                    var rightExp = FormatSubExpression(p, rightResult);

                    if (ShouldBePinnedToFloatingPointVariable(p, arithmeticEvaluationStatement, leftResult,
                        rightResult))
                    {
                        var varName = PinFloatingPointExpressionToVariable(
                            p,
                            dataType,
                            "arithmetic",
                            FormatArithmeticExpression(p,
                                leftResult.DataType,
                                leftExp,
                                arithmeticEvaluationStatement.Operator,
                                rightResult.DataType,
                                rightExp,
                                arithmeticEvaluationStatement
                            ),
                            arithmeticEvaluationStatement
                        );

                        return new ExpressionResult(
                            dataType,
                            varName,
                            new VariableAccessStatement(varName, arithmeticEvaluationStatement.Info)
                        );
                    }

                    var newTmp = new ArithmeticEvaluationStatement(leftResult.Template,
                        arithmeticEvaluationStatement.Operator,
                        rightResult.Template, arithmeticEvaluationStatement.Info);

                    leftResult.Template.ParentStatement = newTmp;
                    rightResult.Template.ParentStatement = newTmp;

                    return new ExpressionResult(
                        dataType,
                        FormatArithmeticExpression(p,
                            leftResult.DataType,
                            leftExp,
                            arithmeticEvaluationStatement.Operator,
                            rightResult.DataType,
                            rightExp,
                            arithmeticEvaluationStatement
                        ),
                        newTmp
                    );
                }

                case AssignmentStatement assignmentStatement:
                {
                    if (!(assignmentStatement.LeftSide is VariableAccessStatement variableAccessStatement))
                    {
                        throw new InvalidStatementStructureCompilerException(assignmentStatement,
                            assignmentStatement.Info);
                    }

                    if (!p.Scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }

                    var transpiler = p.Context.GetTranspilerForStatement(assignmentStatement);
                    transpiler.WriteBlock(p.Context, p.Scope, p.NonInlinePartWriter, p.MetaWriter, assignmentStatement);

                    variableAccessStatement.ParentStatement = assignmentStatement.ParentStatement;

                    return new ExpressionResult(
                        varInfo.DataType,
                        FormatVariableAccessExpression(p, varInfo.DataType, varInfo.AccessName,
                            variableAccessStatement),
                        variableAccessStatement,
                        PinRequiredNotice
                    );
                }

                case FunctionCallStatement functionCallStatement: //functions are always not-inlined.
                {
                    if (!p.Scope.TryGetFunctionInfo(functionCallStatement, out var funcInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                            functionCallStatement.Info);
                    }

                    if (funcInfo.DataType == DataTypes.Void)
                    {
                        if (!(p.UsageContext is BlockStatement))
                        {
                            throw new UsageOfVoidTypeInNonVoidContextCompilerException(functionCallStatement,
                                functionCallStatement.Info);
                        }
                    }

                    if (funcInfo is ApiFunctionInfo apiFunctionInfo)
                    {
                        var result = apiFunctionInfo.Function.Build(p, functionCallStatement);

                        switch (result)
                        {
                            case ApiMethodBuilderRawResult rawResult:
                            {
                                return rawResult.Result;
                            }
                            case ApiMethodBuilderInlineResult inlineResult:
                            {
                                return CreateExpression(p, inlineResult.Statement);
                            }

                            default:
                                throw new InvalidOperationException();
                        }
                    }


                    if (funcInfo.InlinedStatement != null && p.Context.Flags.UseInlining)
                    {
                        var inlined = FunctionInfo.UnWrapInlinedStatement(p.Context, p.Scope, funcInfo);

                        if (inlined is EvaluationStatement evaluationStatement)
                        {
                            return CreateExpressionRecursive(p, evaluationStatement);
                        }

                        if (inlined is ReturnStatement returnStatement)
                        {
                            return CreateExpressionRecursive(p, returnStatement.Result);
                        }

                        //function calls only allowed in blocks, not in evaluation expressions.
                        if (p.UsageContext is BlockStatement)
                        {
                            var transpiler = p.Context.GetTranspilerForStatement(inlined);
                            transpiler.WriteBlock(p.Context, p.Scope, p.NonInlinePartWriter, p.MetaWriter, inlined);
                        }

                        throw new InvalidStatementStructureCompilerException(inlined, inlined.Info);
                    }


                    var call = new StringBuilder(20); //`myfunc 0 "test"`
                    call.Append('`');

                    call.Append(funcInfo.AccessName);

                    var paramTemplates = new List<EvaluationStatement>();
                    foreach (var param in functionCallStatement.Parameters)
                    {
                        var result = CreateExpressionRecursive(p, param);

                        call.Append(' ');
                        call.Append(FormatFunctionCallParameterSubExpression(p, result));

                        paramTemplates.Add(result.Template);
                    }

                    call.Append('`');

                    var callExp = FormatFunctionCallExpression(
                        p,
                        funcInfo.DataType,
                        call.ToString(),
                        functionCallStatement
                    );

                    var paramsArray = paramTemplates.ToArray();
                    var newTmp = new FunctionCallStatement(functionCallStatement.ObjectName,
                        functionCallStatement.FunctionName,
                        funcInfo.DataType, paramsArray, functionCallStatement.Info);

                    foreach (var param in paramsArray)
                    {
                        param.ParentStatement = newTmp;
                    }

                    return new ExpressionResult(
                        funcInfo.DataType,
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