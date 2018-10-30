using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders
{
    public abstract class ExpressionBuilderBase : IExpressionBuilder
    {
        public static readonly IExpressionNotice[] PinRequiredNotice =
        {
            new PinRequiredNotice(),
        };

        public virtual bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor, EvaluationStatement template)
        {
            return false;
        }

        public virtual bool ShouldBePinnedToFloatingPointVariable(
            ExpressionBuilderParams p, EvaluationStatement template, ExpressionResult left, ExpressionResult right)
        {
            return false;
        }

        public virtual bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            EvaluationStatement template, TypeDescriptor left,
            EvaluationStatement leftTemplate, TypeDescriptor right, EvaluationStatement rightTemplate)
        {
            return false;
        }

        public abstract PinnedVariableResult PinExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result);

        public abstract PinnedVariableResult PinExpressionToVariable(ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor, string nameHint,
            string expression, EvaluationStatement template);


        public abstract PinnedVariableResult PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            string nameHint,
            ExpressionResult result);

        public abstract PinnedVariableResult PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor,
            string nameHint, string expression, EvaluationStatement template);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.TypeDescriptor.IsString())
            {
                if (p.FormatString)
                {
                    var value = result.Expression;
                    if (value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        return value;
                    }

                    return $"\"{value}\"";
                }

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
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatArithmeticExpression(ExpressionBuilderParams p, TypeDescriptor leftTypeDescriptor,
            string leftExp,
            IOperator op,
            TypeDescriptor rightTypeDescriptor, string rightExp, EvaluationStatement template)
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
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatBitwiseExpression(ExpressionBuilderParams p, TypeDescriptor leftTypeDescriptor,
            string leftExp,
            IOperator op, TypeDescriptor rightTypeDescriptor, string rightExp, EvaluationStatement template)
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
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual string FormatLogicalExpression(ExpressionBuilderParams p, TypeDescriptor leftTypeDescriptor,
            string leftExp,
            IOperator op, TypeDescriptor rightTypeDescriptor, string rightExp, EvaluationStatement template)
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
        public virtual string FormatVariableAccessExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
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
        public virtual string FormatFunctionCallExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
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
        public virtual string FormatConstantExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
            string expression,
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

            if (result.IsEmptyResult)
            {
                return result;
            }

            return new ExpressionResult(
                result.TypeDescriptor,
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

                            var template = new VariableAccessStatement(varName.Name, left.Template.Info);

                            left = new ExpressionResult(left.TypeDescriptor, varName.Expression, template);

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
                        constantValueStatement.TypeDescriptor,
                        FormatConstantExpression(p,
                            constantValueStatement.TypeDescriptor,
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
                            varInfo.TypeDescriptor,
                            FormatVariableAccessExpression(p,
                                varInfo.TypeDescriptor,
                                varInfo.AccessName,
                                variableAccessStatement
                            ),
                            variableAccessStatement
                        );
                    }

                    if (p.Scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                    {
                        var type = constInfo.TypeDescriptor;

                        var constantValueStatement =
                            new ConstantValueStatement(type, constInfo.Value, variableAccessStatement.Info);
                        //should be impossible to reach.
                        return new ExpressionResult(
                            type,
                            FormatConstantExpression(p,
                                type,
                                type.IsBoolean()
                                    ? constInfo.Value.ToLower()
                                    : constInfo.Value,
                                constantValueStatement
                            ),
                            constantValueStatement
                        );
                    }

                    if (p.Scope.TryGetPrototypeInfo(variableAccessStatement, out var functionInfo) ||
                        p.Scope.TryGetFunctionInfo(variableAccessStatement, out functionInfo))
                    {
                        var type = new TypeDescriptor(DataTypes.Delegate,
                            new TypeDescriptor.LookupInfo(functionInfo.ClassName, functionInfo.Name));

                        var constantValueStatement =
                            new ConstantValueStatement(type, functionInfo.AccessName, variableAccessStatement.Info);

                        return new ExpressionResult(
                            type,
                            FormatConstantExpression(p,
                                type,
                                functionInfo.AccessName,
                                constantValueStatement
                            ),
                            constantValueStatement
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

                        if (!result.TypeDescriptor.IsInteger())
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
                                TypeDescriptor.Integer,
                                FormatBitwiseExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
                    }

                    var leftResult = CreateExpressionRecursive(p, bitwiseEvaluationStatement.Left);

                    if (!(leftResult.TypeDescriptor.IsInteger() || leftResult.TypeDescriptor.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(leftResult.Template, leftResult.Template.Info);
                    }

                    var rightResult = CreateExpressionRecursive(p, bitwiseEvaluationStatement.Right);

                    if (!(rightResult.TypeDescriptor.IsInteger() || rightResult.TypeDescriptor.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(bitwiseEvaluationStatement,
                            bitwiseEvaluationStatement.Info);
                    }

                    HandleNotices(p, ref leftResult, ref rightResult);

                    if (leftResult.TypeDescriptor != rightResult.TypeDescriptor)
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
                            leftResult.TypeDescriptor,
                            FormatBitwiseExpression(p,
                                leftResult.TypeDescriptor,
                                leftExp,
                                bitwiseEvaluationStatement.Operator,
                                rightResult.TypeDescriptor,
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

                        if (!result.TypeDescriptor.IsBoolean())
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }

                        var exp = FormatSubExpression(p, result);

                        exp = $"! {exp}";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.TypeDescriptor,
                            logicalEvaluationStatement))
                        {
                            var varName = PinFloatingPointExpressionToVariable(
                                p,
                                result.TypeDescriptor,
                                null,
                                FormatLogicalExpression(p,
                                    exp,
                                    logicalEvaluationStatement
                                ),
                                logicalEvaluationStatement
                            );
                            return new ExpressionResult(
                                varName.TypeDescriptor,
                                varName.Expression,
                                varName.Template
                            );
                        }

                        {
                            var newTemp = new LogicalEvaluationStatement(null, logicalEvaluationStatement.Operator,
                                result.Template, logicalEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                TypeDescriptor.Boolean,
                                FormatLogicalExpression(p,
                                    exp,
                                    newTemp
                                ),
                                newTemp
                            );
                        }
                    }

                    var leftResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Left);

                    if (!(leftResult.TypeDescriptor.IsNumber() || leftResult.TypeDescriptor.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                            logicalEvaluationStatement.Info);
                    }

                    var rightResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Right);

                    if (!(rightResult.TypeDescriptor.IsNumber() || rightResult.TypeDescriptor.IsBoolean()))
                    {
                        throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                            logicalEvaluationStatement.Info);
                    }

                    HandleNotices(p, ref leftResult, ref rightResult);

                    if (leftResult.TypeDescriptor != rightResult.TypeDescriptor)
                    {
                        //TODO: fix if you want to support string comparison or etc. (i.e. "test" > "TEST")
                        if (!(leftResult.TypeDescriptor.IsNumber() && rightResult.TypeDescriptor.IsNumber()))
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }
                    }
                    else
                    {
                        var dataType = leftResult.TypeDescriptor.DataType;
                        if (dataType.IsDelegate() || dataType.IsObject() || dataType.IsArray())
                        {
                            throw new InvalidStatementCompilerException(logicalEvaluationStatement,
                                logicalEvaluationStatement.Info);
                        }
                    }

                    var leftExp = FormatSubExpression(p, leftResult);
                    var rightExp = FormatSubExpression(p, rightResult);

                    if (ShouldBePinnedToFloatingPointVariable(p, logicalEvaluationStatement, leftResult, rightResult))
                    {
                        return PinFloatingPointExpressionToVariable(
                            p,
                            TypeDescriptor.Boolean,
                            "logical",
                            FormatLogicalExpression(p,
                                leftResult,
                                logicalEvaluationStatement.Operator,
                                rightResult,
                                logicalEvaluationStatement
                            ),
                            logicalEvaluationStatement
                        );
                    }

                    {
                        var newTemp = new LogicalEvaluationStatement(leftResult.Template,
                            logicalEvaluationStatement.Operator,
                            rightResult.Template, logicalEvaluationStatement.Info);

                        leftResult.Template.ParentStatement = newTemp;
                        rightResult.Template.ParentStatement = newTemp;


                        return new ExpressionResult(
                            TypeDescriptor.Boolean,
                            FormatLogicalExpression(p,
                                leftResult.TypeDescriptor,
                                leftExp,
                                logicalEvaluationStatement.Operator,
                                rightResult.TypeDescriptor,
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
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var result = CreateExpressionRecursive(p, operand);

                        if (!result.TypeDescriptor.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var exp = arithmeticEvaluationStatement.Left == null
                            ? $"++{result.Expression}"
                            : $"{result.Expression}++";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.TypeDescriptor,
                            arithmeticEvaluationStatement))
                        {
                            return PinFloatingPointExpressionToVariable(
                                p,
                                result.TypeDescriptor,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
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
                                result.TypeDescriptor,
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
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var result = CreateExpressionRecursive(p, operand);

                        if (!result.TypeDescriptor.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var exp = arithmeticEvaluationStatement.Left == null
                            ? $"--{result.Expression}"
                            : $"{result.Expression}--";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.TypeDescriptor,
                            arithmeticEvaluationStatement))
                        {
                            return PinFloatingPointExpressionToVariable(
                                p,
                                result.TypeDescriptor,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
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
                                result.TypeDescriptor,
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

                        if (!result.TypeDescriptor.IsNumber())
                        {
                            throw new InvalidStatementCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }

                        var exp = FormatSubExpression(p, result);

                        exp = $"-{exp}";

                        if (ShouldBePinnedToFloatingPointVariable(p, result.TypeDescriptor,
                            arithmeticEvaluationStatement))
                        {
                            return PinFloatingPointExpressionToVariable(
                                p,
                                result.TypeDescriptor,
                                null,
                                FormatArithmeticExpression(p,
                                    exp,
                                    arithmeticEvaluationStatement
                                ),
                                arithmeticEvaluationStatement
                            );
                        }

                        {
                            var newTemp = new ArithmeticEvaluationStatement(null,
                                arithmeticEvaluationStatement.Operator, result.Template,
                                arithmeticEvaluationStatement.Info);

                            result.Template.ParentStatement = newTemp;

                            return new ExpressionResult(
                                result.TypeDescriptor,
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

                    if ((leftResult.TypeDescriptor.IsString() || rightResult.TypeDescriptor.IsString()) &&
                        !(arithmeticEvaluationStatement.Operator is AdditionOperator))
                    {
                        throw new InvalidOperatorForTypeCompilerException(
                            arithmeticEvaluationStatement.Operator.GetType(), leftResult.TypeDescriptor,
                            rightResult.TypeDescriptor,
                            arithmeticEvaluationStatement.Info);
                    }

                    var dataType = StatementHelpers.OperateDataTypes(arithmeticEvaluationStatement.Operator,
                        leftResult.TypeDescriptor, rightResult.TypeDescriptor);

                    var leftExp = FormatSubExpression(p, leftResult);
                    var rightExp = FormatSubExpression(p, rightResult);

                    if (ShouldBePinnedToFloatingPointVariable(p, arithmeticEvaluationStatement, leftResult,
                        rightResult))
                    {
                        return PinFloatingPointExpressionToVariable(
                            p,
                            dataType,
                            "arithmetic",
                            FormatArithmeticExpression(p,
                                leftResult.TypeDescriptor,
                                leftExp,
                                arithmeticEvaluationStatement.Operator,
                                rightResult.TypeDescriptor,
                                rightExp,
                                arithmeticEvaluationStatement
                            ),
                            arithmeticEvaluationStatement
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
                            leftResult.TypeDescriptor,
                            leftExp,
                            arithmeticEvaluationStatement.Operator,
                            rightResult.TypeDescriptor,
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
                        varInfo.TypeDescriptor,
                        FormatVariableAccessExpression(p, varInfo.TypeDescriptor, varInfo.AccessName,
                            variableAccessStatement),
                        variableAccessStatement,
                        PinRequiredNotice
                    );
                }

                case FunctionCallStatement functionCallStatement: //functions are always not-inlined.
                {
                    var funcInfo =
                        FunctionStatementTranspilerBase.GetFunctionInfoFromFunctionCall(p.Context, p.Scope,
                            functionCallStatement, out var sourceObjectInfo);

                    if (funcInfo.TypeDescriptor.IsVoid())
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
                        var inlined = UnWrapInlinedStatement(p, funcInfo, functionCallStatement);

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

                            return ExpressionResult.EmptyResult;
                        }

                        throw new InvalidStatementStructureCompilerException(inlined, inlined.Info);
                    }


                    var call = new StringBuilder(20); //`myfunc 0 "test"`
                    if (!p.VoidFunctionCall)
                    {
                        call.Append('`');
                    }

                    var accessName = sourceObjectInfo.AccessName;

                    if (sourceObjectInfo is VariableInfo variableInfo)
                    {
                        accessName =
                            FormatVariableAccessExpression(p, variableInfo.TypeDescriptor, accessName,
                                new VariableAccessStatement(variableInfo.ClassName, variableInfo.Name,
                                    functionCallStatement.Info));
                    }

                    call.Append(accessName);

                    var paramTemplates = new List<EvaluationStatement>();
                    foreach (var param in functionCallStatement.Parameters)
                    {
                        var result = CreateExpressionRecursive(p, param);

                        call.Append(' ');
                        call.Append(FormatFunctionCallParameterSubExpression(p, result));

                        paramTemplates.Add(result.Template);
                    }

                    if (!p.VoidFunctionCall)
                    {
                        call.Append('`');
                    }

                    var callExp = FormatFunctionCallExpression(
                        p,
                        funcInfo.TypeDescriptor,
                        call.ToString(),
                        functionCallStatement
                    );

                    var paramsArray = paramTemplates.ToArray();
                    var newTmp = new FunctionCallStatement(functionCallStatement.ClassName,
                        functionCallStatement.FunctionName,
                        funcInfo.TypeDescriptor, paramsArray, functionCallStatement.Info);

                    foreach (var param in paramsArray)
                    {
                        param.ParentStatement = newTmp;
                    }

                    return new ExpressionResult(
                        funcInfo.TypeDescriptor,
                        callExp,
                        newTmp
                    );
                }

                default:
                    throw new InvalidOperationException();
            }
        }

        protected virtual IStatement UnWrapInlinedStatement(ExpressionBuilderParams p, FunctionInfo functionInfo,
            FunctionCallStatement functionCallStatement)
        {
            var inlined = functionInfo.InlinedStatement;

            if (inlined == null)
            {
                return null;
            }

            var schemeParameters = functionInfo.Parameters;

            if (schemeParameters != null && schemeParameters.Length > 0)
            {
                var nameMapping = new Dictionary<string, EvaluationStatement>();

                for (var i = 0; i < schemeParameters.Length; i++)
                {
                    var param = FunctionStatementTranspilerBase.GetSchemeParameterValueByIndex(p.Context, p.Scope,
                        functionInfo, functionCallStatement, i);

                    switch (param)
                    {
                        case ConstantValueStatement _:
                        case VariableAccessStatement _:
                            break;
                        default:
                        {
                            var pName = schemeParameters[i].Name;

                            if (inlined.TreeCount(x =>
                                    x is VariableAccessStatement variableAccessStatement &&
                                    variableAccessStatement.VariableName == pName) > 1)
                            {
                                var result = CreateExpression(p, param);

                                var varName = PinExpressionToVariable(p, schemeParameters[i].Name, result);

                                param = varName.Template;
                            }

                            break;
                        }
                    }

                    nameMapping.Add(schemeParameters[i].Name, param);
                }

                FunctionStatementTranspilerBase.ReplaceEvaluation(inlined, key => nameMapping.ContainsKey(key),
                    key => nameMapping[key], out inlined);
            }

            if (inlined is FunctionCallStatement funcCallStt)
            {
                if (p.Scope.TryGetFunctionInfo(funcCallStt, out functionInfo))
                {
                    inlined = UnWrapInlinedStatement(p, functionInfo, funcCallStt);
                }
            }

            return inlined;
        }
    }
}