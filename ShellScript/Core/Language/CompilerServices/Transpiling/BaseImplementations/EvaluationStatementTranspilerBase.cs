using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class EvaluationStatementTranspilerBase : StatementTranspilerBase,
        IPlatformEvaluationStatementTranspiler
    {
        public override Type StatementType => typeof(EvaluationStatement);


        public static bool CanInlineEvaluation(Context context, Scope scope, EvaluationStatement statement)
        {
            return true;
            //return !statement.TreeContains<FunctionCallStatement>();
        }

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return true;
            //if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            //return !evalStt.TreeContains<FunctionCallStatement>();
        }

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            StatementInfo info = null;
            string identifierName = null;

            //check for all variables defined.
            if (evalStt.TreeAny(stt =>
            {
                if (stt is VariableAccessStatement varAccessStt)
                {
                    identifierName = varAccessStt.VariableName;
                    info = varAccessStt.Info;
                    return !scope.IsIdentifierExists(varAccessStt.VariableName);
                }

                if (stt is FunctionCallStatement functionCallStatement)
                {
                    identifierName = functionCallStatement.Fqn;
                    info = functionCallStatement.Info;
                    return !scope.IsIdentifierExists(functionCallStatement);
                }

                return false;
            }))
            {
                message = IdentifierNotFoundCompilerException.CreateMessage(identifierName, info);
                return false;
            }

            return base.Validate(context, scope, statement, out message);
        }

        public abstract string PinEvaluationToVariable(Context context, Scope scope, TextWriter metaWriter,
            TextWriter pinCodeWriter, EvaluationStatement statement);

        public abstract ExpressionResult GetExpression(ExpressionBuilderParams p,
            EvaluationStatement statement);

        public abstract ExpressionResult GetExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext,
            EvaluationStatement statement);

        public abstract ExpressionResult GetConditionalExpression(ExpressionBuilderParams p,
            EvaluationStatement statement);

        public abstract ExpressionResult GetConditionalExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext,
            EvaluationStatement statement);


        public static EvaluationStatement ProcessEvaluation(Context context, Scope scope,
            EvaluationStatement statement)
        {
            if (statement == null)
            {
                throw BashTranspilerHelpers.InvalidStatementStructure(scope, null);
            }

            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    if (constantValueStatement.IsNumber())
                    {
                        if (constantValueStatement.IsDecimal())
                        {
                            if (long.TryParse(constantValueStatement.Value, out _))
                            {
                                return constantValueStatement;
                            }
                        }
                        else if (long.TryParse(constantValueStatement.Value, out _))
                        {
                            return new ConstantValueStatement(DataTypes.Decimal, constantValueStatement.Value,
                                constantValueStatement.Info);
                        }

                        if (double.TryParse(constantValueStatement.Value, out _))
                        {
                            return constantValueStatement;
                        }

                        throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                            constantValueStatement);
                    }

                    if (constantValueStatement.IsString())
                    {
                        var str = StatementHelpers.UnEscapeString(constantValueStatement.Value);
                        
                        return new ConstantValueStatement(DataTypes.String, str, constantValueStatement.Info);
                    }

                    return constantValueStatement;
                }

                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement, out _))
                    {
                        return variableAccessStatement;
                    }

                    if (scope.TryGetConstantInfo(variableAccessStatement, out var constInfo))
                    {
                        return new ConstantValueStatement(constInfo.DataType, constInfo.Value,
                            variableAccessStatement.Info);
                    }

                    throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                        variableAccessStatement.Info);
                }

                case BitwiseEvaluationStatement bitwiseEvaluationStatement:
                {
                    switch (bitwiseEvaluationStatement.Operator)
                    {
                        case BitwiseNotOperator _:
                        {
                            if (bitwiseEvaluationStatement.Left != null)
                            {
                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    bitwiseEvaluationStatement);
                            }

                            if (bitwiseEvaluationStatement.Right is ConstantValueStatement constantValueStatement)
                            {
                                if (constantValueStatement.DataType == DataTypes.Decimal)
                                {
                                    if (!long.TryParse(constantValueStatement.Value, out var value))
                                    {
                                        throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                            bitwiseEvaluationStatement);
                                    }

                                    return new ConstantValueStatement(
                                        constantValueStatement.DataType,
                                        (~value).ToString(NumberFormatInfo.InvariantInfo),
                                        constantValueStatement.Info
                                    )
                                    {
                                        ParentStatement = bitwiseEvaluationStatement.ParentStatement
                                    };
                                }

                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    bitwiseEvaluationStatement);
                            }

                            throw BashTranspilerHelpers.InvalidStatementStructure(scope, bitwiseEvaluationStatement);
                        }
                        case BitwiseAndOperator _:
                        case BitwiseOrOperator _:
                        case XorOperator _:
                        {
                            var left = ProcessEvaluation(context, scope, bitwiseEvaluationStatement.Left);
                            var right = ProcessEvaluation(context, scope, bitwiseEvaluationStatement.Right);

                            if (left is ConstantValueStatement leftConstantValue &&
                                right is ConstantValueStatement rightConstantValue)
                            {
                                if (leftConstantValue.IsDecimal() &&
                                    long.TryParse(leftConstantValue.Value, out var leftDecimal))
                                {
                                    if (rightConstantValue.IsDecimal() &&
                                        long.TryParse(rightConstantValue.Value, out var rightDecimal))
                                    {
                                        long resultVal;
                                        switch (bitwiseEvaluationStatement.Operator)
                                        {
                                            case BitwiseAndOperator _:
                                                resultVal = leftDecimal & rightDecimal;
                                                break;
                                            case BitwiseOrOperator _:
                                                resultVal = leftDecimal | rightDecimal;
                                                break;
                                            case XorOperator _:
                                                resultVal = leftDecimal ^ rightDecimal;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Decimal,
                                            resultVal.ToString(CultureInfo.InvariantCulture),
                                            bitwiseEvaluationStatement.Info)
                                        {
                                            ParentStatement = bitwiseEvaluationStatement.ParentStatement
                                        };
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        bitwiseEvaluationStatement.Operator.GetType(), rightConstantValue.DataType,
                                        rightConstantValue.Info);
                                }

                                if (leftConstantValue.DataType == DataTypes.Boolean &&
                                    rightConstantValue.DataType == DataTypes.Boolean &&
                                    bool.TryParse(leftConstantValue.Value, out var leftBool) &&
                                    bool.TryParse(rightConstantValue.Value, out var rightBool))
                                {
                                    bool resultVal;
                                    switch (bitwiseEvaluationStatement.Operator)
                                    {
                                        case BitwiseAndOperator _:
                                            resultVal = leftBool && rightBool;
                                            break;
                                        case BitwiseOrOperator _:
                                            resultVal = leftBool || rightBool;
                                            break;
                                        case XorOperator _:
                                            resultVal = leftBool ^ rightBool;
                                            break;
                                        default:
                                            throw new InvalidOperationException();
                                    }

                                    return new ConstantValueStatement(DataTypes.Boolean,
                                        resultVal.ToString(CultureInfo.InvariantCulture),
                                        bitwiseEvaluationStatement.Info)
                                    {
                                        ParentStatement = bitwiseEvaluationStatement.ParentStatement
                                    };
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    bitwiseEvaluationStatement.Operator.GetType(), leftConstantValue.DataType,
                                    leftConstantValue.Info);
                            }

                            var result = new BitwiseEvaluationStatement(
                                left,
                                bitwiseEvaluationStatement.Operator,
                                right,
                                bitwiseEvaluationStatement.Info)
                            {
                                ParentStatement = bitwiseEvaluationStatement.ParentStatement
                            };

                            left.ParentStatement = result;
                            right.ParentStatement = result;

                            return result;
                        }

                        default:
                            throw new InvalidOperationException();
                    }
                }
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    switch (logicalEvaluationStatement.Operator)
                    {
                        case LogicalAndOperator _:
                        case LogicalOrOperator _:
                        {
                            var left = ProcessEvaluation(context, scope, logicalEvaluationStatement.Left);
                            var right = ProcessEvaluation(context, scope, logicalEvaluationStatement.Right);

                            var leftConstantValue = left as ConstantValueStatement;
                            var rightConstantValue = right as ConstantValueStatement;

                            ValueTuple<ConstantValueStatement, EvaluationStatement> singleConstantPair;

                            if (leftConstantValue != null)
                            {
                                if (rightConstantValue != null)
                                {
                                    if ((leftConstantValue.IsBoolean() /*||
                                         leftConstantValue.IsDecimal()*/) &&
                                        StatementHelpers.TryParseBooleanFromString(leftConstantValue.Value,
                                            out var leftBool))
                                    {
                                        if ((rightConstantValue.IsBoolean() /*||
                                             rightConstantValue.IsDecimal()*/) &&
                                            StatementHelpers.TryParseBooleanFromString(rightConstantValue.Value,
                                                out var rightBool))
                                        {
                                            return new ConstantValueStatement(DataTypes.Boolean,
                                                (logicalEvaluationStatement.Operator is LogicalAndOperator
                                                    ? leftBool && rightBool
                                                    : leftBool || rightBool).ToString(CultureInfo.InvariantCulture),
                                                logicalEvaluationStatement.Info)
                                            {
                                                ParentStatement = logicalEvaluationStatement.ParentStatement
                                            };
                                        }

                                        throw new InvalidOperatorForTypeCompilerException(
                                            logicalEvaluationStatement.Operator.GetType(), rightConstantValue.DataType,
                                            rightConstantValue.Info);
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        logicalEvaluationStatement.Operator.GetType(), leftConstantValue.DataType,
                                        leftConstantValue.Info);
                                }

                                singleConstantPair.Item1 = leftConstantValue;
                                singleConstantPair.Item2 = right;
                            }
                            else
                            {
                                singleConstantPair.Item1 = rightConstantValue;
                                singleConstantPair.Item2 = left;
                            }

                            if (singleConstantPair.Item1 != null)
                            {
                                if (singleConstantPair.Item1.IsBoolean())
                                {
                                    if (StatementHelpers.TryParseBooleanFromString(singleConstantPair.Item1.Value,
                                        out var boolVal))
                                    {
                                        if (logicalEvaluationStatement.Operator is LogicalAndOperator)
                                        {
                                            return boolVal
                                                ? singleConstantPair.Item2
                                                : singleConstantPair.Item1;
                                        }

                                        //LogicalOrOperator
                                        return boolVal
                                            ? singleConstantPair.Item1
                                            : singleConstantPair.Item2;
                                    }

                                    throw new InvalidStatementStructureCompilerException(singleConstantPair.Item1,
                                        singleConstantPair.Item1.Info);
                                }

                                throw new InvalidStatementStructureCompilerException(singleConstantPair.Item1,
                                    singleConstantPair.Item1.Info);
                            }

                            var result = new LogicalEvaluationStatement(
                                left,
                                logicalEvaluationStatement.Operator,
                                right,
                                logicalEvaluationStatement.Info)
                            {
                                ParentStatement = logicalEvaluationStatement.ParentStatement
                            };

                            left.ParentStatement = result;
                            right.ParentStatement = result;

                            return result;
                        }
                        case EqualOperator _:
                        case NotEqualOperator _:
                        {
                            var left = ProcessEvaluation(context, scope, logicalEvaluationStatement.Left);
                            var right = ProcessEvaluation(context, scope, logicalEvaluationStatement.Right);

                            if (left is ConstantValueStatement leftConstant &&
                                right is ConstantValueStatement rightConstant)
                            {
                                if (leftConstant.IsNumber() && rightConstant.IsNumber())
                                {
                                    if (leftConstant.IsDecimal() && rightConstant.IsDecimal() &&
                                        long.TryParse(leftConstant.Value, out var leftDecimal) &&
                                        long.TryParse(rightConstant.Value, out var rightDecimal)
                                    )
                                    {
                                        return new ConstantValueStatement(
                                            DataTypes.Boolean,
                                            (logicalEvaluationStatement.Operator is EqualOperator
                                                ? leftDecimal == rightDecimal
                                                : leftDecimal != rightDecimal).ToString(CultureInfo.InvariantCulture),
                                            logicalEvaluationStatement.Info
                                        )
                                        {
                                            ParentStatement = logicalEvaluationStatement.ParentStatement
                                        };
                                    }

                                    if (double.TryParse(leftConstant.Value, out var leftFloat) &&
                                        double.TryParse(rightConstant.Value, out var rightFloat))
                                    {
                                        return new ConstantValueStatement(
                                            DataTypes.Boolean,
                                            (Math.Abs(leftFloat - rightFloat) < double.Epsilon).ToString(CultureInfo
                                                .InvariantCulture),
                                            logicalEvaluationStatement.Info
                                        )
                                        {
                                            ParentStatement = logicalEvaluationStatement.ParentStatement
                                        };
                                    }
                                }

                                return new ConstantValueStatement(
                                    DataTypes.Boolean,
                                    (logicalEvaluationStatement.Operator is EqualOperator
                                        ? leftConstant.Value == rightConstant.Value
                                        : leftConstant.Value != rightConstant.Value)
                                    .ToString(CultureInfo.InvariantCulture),
                                    logicalEvaluationStatement.Info
                                )
                                {
                                    ParentStatement = logicalEvaluationStatement.ParentStatement
                                };
                            }

                            var result = new LogicalEvaluationStatement(
                                left,
                                logicalEvaluationStatement.Operator,
                                right,
                                logicalEvaluationStatement.Info)
                            {
                                ParentStatement = logicalEvaluationStatement.ParentStatement
                            };

                            left.ParentStatement = result;
                            right.ParentStatement = result;

                            return result;
                        }
                        case GreaterOperator _:
                        case GreaterEqualOperator _:
                        case LessOperator _:
                        case LessEqualOperator _:
                        {
                            var left = ProcessEvaluation(context, scope, logicalEvaluationStatement.Left);
                            var right = ProcessEvaluation(context, scope, logicalEvaluationStatement.Right);

                            if (left is ConstantValueStatement leftConstantValue &&
                                right is ConstantValueStatement rightConstantValue)
                            {
                                if (leftConstantValue.IsNumber() && rightConstantValue.IsNumber())
                                {
                                    if (leftConstantValue.IsDecimal() && rightConstantValue.IsDecimal() &&
                                        long.TryParse(leftConstantValue.Value, out var leftDecimal) &&
                                        long.TryParse(rightConstantValue.Value, out var rightDecimal))
                                    {
                                        bool resultVal;
                                        switch (logicalEvaluationStatement.Operator)
                                        {
                                            case GreaterOperator _:
                                                resultVal = leftDecimal > rightDecimal;
                                                break;
                                            case GreaterEqualOperator _:
                                                resultVal = leftDecimal >= rightDecimal;
                                                break;
                                            case LessOperator _:
                                                resultVal = leftDecimal < rightDecimal;
                                                break;
                                            case LessEqualOperator _:
                                                resultVal = leftDecimal <= rightDecimal;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Boolean,
                                            resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                            logicalEvaluationStatement.Info)
                                        {
                                            ParentStatement = logicalEvaluationStatement.ParentStatement
                                        };
                                    }

                                    if (double.TryParse(leftConstantValue.Value, out var leftFloat) &&
                                        double.TryParse(rightConstantValue.Value, out var rightFloat))
                                    {
                                        bool resultVal;
                                        switch (logicalEvaluationStatement.Operator)
                                        {
                                            case GreaterOperator _:
                                                resultVal = leftFloat > rightFloat;
                                                break;
                                            case GreaterEqualOperator _:
                                                resultVal = leftFloat >= rightFloat;
                                                break;
                                            case LessOperator _:
                                                resultVal = leftFloat < rightFloat;
                                                break;
                                            case LessEqualOperator _:
                                                resultVal = leftFloat <= rightFloat;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Boolean,
                                            resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                            logicalEvaluationStatement.Info)
                                        {
                                            ParentStatement = logicalEvaluationStatement.ParentStatement
                                        };
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        logicalEvaluationStatement.Operator.GetType(), rightConstantValue.DataType,
                                        rightConstantValue.Info);
                                }

                                if (leftConstantValue.IsString() && rightConstantValue.IsString())
                                {
                                    bool resultVal;
                                    var comp = context.StringComparer.Compare(leftConstantValue.Value,
                                        rightConstantValue.Value);

                                    switch (logicalEvaluationStatement.Operator)
                                    {
                                        case GreaterOperator _:
                                            resultVal = comp > 0;
                                            break;
                                        case GreaterEqualOperator _:
                                            resultVal = comp >= 0;
                                            break;
                                        case LessOperator _:
                                            resultVal = comp < 0;
                                            break;
                                        case LessEqualOperator _:
                                            resultVal = comp <= 0;
                                            break;
                                        default:
                                            throw new InvalidOperationException();
                                    }

                                    return new ConstantValueStatement(DataTypes.Boolean,
                                        resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                        logicalEvaluationStatement.Info)
                                    {
                                        ParentStatement = logicalEvaluationStatement.ParentStatement
                                    };
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    logicalEvaluationStatement.Operator.GetType(), leftConstantValue.DataType,
                                    leftConstantValue.Info);
                            }

                            var result = new LogicalEvaluationStatement(
                                left,
                                logicalEvaluationStatement.Operator,
                                right,
                                logicalEvaluationStatement.Info)
                            {
                                ParentStatement = logicalEvaluationStatement.ParentStatement
                            };

                            left.ParentStatement = result;
                            right.ParentStatement = result;

                            return result;
                        }
                        case NotOperator _:
                        {
                            if (logicalEvaluationStatement.Left != null)
                            {
                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    logicalEvaluationStatement);
                            }

                            if (logicalEvaluationStatement.Right is ConstantValueStatement constantValueStatement)
                            {
                                if (constantValueStatement.DataType == DataTypes.Boolean ||
                                    constantValueStatement.IsNumber())
                                {
                                    if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value,
                                        out var value))
                                    {
                                        throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                            logicalEvaluationStatement);
                                    }

                                    return new ConstantValueStatement(
                                        constantValueStatement.DataType,
                                        (!value).ToString(NumberFormatInfo.InvariantInfo),
                                        constantValueStatement.Info
                                    )
                                    {
                                        ParentStatement = logicalEvaluationStatement.ParentStatement
                                    };
                                }

                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    logicalEvaluationStatement);
                            }

                            return logicalEvaluationStatement;
                            //throw BashTranspilerHelpers.InvalidStatementStructure(scope, logicalEvaluationStatement);
                        }
                        default:
                            throw new InvalidOperationException();
                    }
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case IncrementOperator _:
                        case DecrementOperator _:
                        {
                            var left = arithmeticEvaluationStatement.Left;
                            var right = arithmeticEvaluationStatement.Right;

                            if (left != null && right != null)
                            {
                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    arithmeticEvaluationStatement);
                            }

                            var operand = left ?? right;
                            operand = ProcessEvaluation(context, scope, operand);
                            switch (operand)
                            {
                                case VariableAccessStatement variableAccessStatement:
                                {
                                    if (scope.TryGetVariableInfo(variableAccessStatement, out _))
                                    {
                                        variableAccessStatement.ParentStatement =
                                            arithmeticEvaluationStatement.ParentStatement;

                                        return variableAccessStatement;
                                    }

                                    if (scope.TryGetConstantInfo(variableAccessStatement, out _))
                                    {
                                        throw new InvalidOperatorForTypeCompilerException(
                                            InvalidOperatorForTypeCompilerException.CreateMessageForConstant(
                                                arithmeticEvaluationStatement.Operator.GetType()),
                                            arithmeticEvaluationStatement.Info);
                                    }

                                    throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                                        variableAccessStatement.Info);
                                }
                                default:
                                    throw new InvalidOperatorForTypeCompilerException(
                                        InvalidOperatorForTypeCompilerException.CreateMessageForConstant(
                                            arithmeticEvaluationStatement.Operator.GetType()),
                                        arithmeticEvaluationStatement.Info);
                            }
                        }
                        case NegativeNumberOperator _:
                        {
                            var right = ProcessEvaluation(context, scope, arithmeticEvaluationStatement.Right);

                            if (right == null || arithmeticEvaluationStatement.Left != null)
                            {
                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    arithmeticEvaluationStatement);
                            }

                            if (right is ConstantValueStatement constantValueStatement)
                            {
                                if (constantValueStatement.IsNumber())
                                {
                                    if (long.TryParse(constantValueStatement.Value, out var longResult))
                                    {
                                        return new ConstantValueStatement(
                                            constantValueStatement.DataType,
                                            (-longResult).ToString(NumberFormatInfo.InvariantInfo),
                                            constantValueStatement.Info
                                        );
                                    }

                                    if (double.TryParse(constantValueStatement.Value, out var doubleResult))
                                    {
                                        return new ConstantValueStatement(
                                            constantValueStatement.DataType,
                                            (-doubleResult).ToString(NumberFormatInfo.InvariantInfo),
                                            constantValueStatement.Info
                                        )
                                        {
                                            ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                        };
                                    }
                                }

                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    arithmeticEvaluationStatement);
                            }

                            var result = new ArithmeticEvaluationStatement(
                                null,
                                arithmeticEvaluationStatement.Operator,
                                right,
                                arithmeticEvaluationStatement.Info)
                            {
                                ParentStatement = arithmeticEvaluationStatement.ParentStatement
                            };

                            right.ParentStatement = result;

                            return result;
                        }
                        case AdditionOperator _:
                        case SubtractionOperator _:
                        case MultiplicationOperator _:
                        case DivisionOperator _:
                        case ModulusOperator _:
                        //case ReminderOperator _:
                        {
                            var left = ProcessEvaluation(context, scope, arithmeticEvaluationStatement.Left);
                            var right = ProcessEvaluation(context, scope, arithmeticEvaluationStatement.Right);

                            if (left is ConstantValueStatement leftConstant &&
                                right is ConstantValueStatement rightConstant)
                            {
                                if (leftConstant.IsNumber() && rightConstant.IsNumber())
                                {
                                    if (leftConstant.IsDecimal() && rightConstant.IsDecimal() &&
                                        long.TryParse(leftConstant.Value, out var leftDecimal) &&
                                        long.TryParse(rightConstant.Value, out var rightDecimal))
                                    {
                                        long resultVal;
                                        switch (arithmeticEvaluationStatement.Operator)
                                        {
                                            case AdditionOperator _:
                                                resultVal = leftDecimal + rightDecimal;
                                                break;
                                            case SubtractionOperator _:
                                                resultVal = leftDecimal - rightDecimal;
                                                break;
                                            case MultiplicationOperator _:
                                                resultVal = leftDecimal * rightDecimal;
                                                break;
                                            case DivisionOperator _:
                                                resultVal = leftDecimal / rightDecimal;
                                                break;
                                            case ModulusOperator _:
                                                resultVal = leftDecimal % rightDecimal;
                                                break;
                                            //case ReminderOperator _:
                                            //    resultVal = leftDecimal % rightDecimal;
                                            //    break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Decimal,
                                            resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                            arithmeticEvaluationStatement.Info
                                        )
                                        {
                                            ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                        };
                                    }

                                    if (double.TryParse(leftConstant.Value, out var leftFloat) &&
                                        double.TryParse(rightConstant.Value, out var rightFloat))
                                    {
                                        double resultVal;
                                        switch (arithmeticEvaluationStatement.Operator)
                                        {
                                            case AdditionOperator _:
                                                resultVal = leftFloat + rightFloat;
                                                break;
                                            case SubtractionOperator _:
                                                resultVal = leftFloat - rightFloat;
                                                break;
                                            case MultiplicationOperator _:
                                                resultVal = leftFloat * rightFloat;
                                                break;
                                            case DivisionOperator _:
                                                resultVal = leftFloat / rightFloat;
                                                break;
                                            case ModulusOperator _:
                                                resultVal = leftFloat % rightFloat;
                                                break;
                                            //case ReminderOperator _:
                                            //    resultVal = leftFloat % rightFloat;
                                            //    break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Decimal,
                                            resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                            arithmeticEvaluationStatement.Info
                                        )
                                        {
                                            ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                        };
                                    }

                                    throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                        arithmeticEvaluationStatement);
                                }

                                if (leftConstant.IsString() && rightConstant.IsString() &&
                                    arithmeticEvaluationStatement.Operator is AdditionOperator)
                                {
                                    return new ConstantValueStatement(DataTypes.String,
                                        BashTranspilerHelpers.StandardizeString(leftConstant.Value, true) +
                                        BashTranspilerHelpers.StandardizeString(rightConstant.Value, true),
                                        arithmeticEvaluationStatement.Info
                                    )
                                    {
                                        ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                    };
                                }

                                if (leftConstant.IsDecimal() || rightConstant.IsString())
                                {
                                    if (!int.TryParse(leftConstant.Value, out var count))
                                    {
                                        throw BashTranspilerHelpers.InvalidStatementStructure(scope, leftConstant);
                                    }

                                    var str = BashTranspilerHelpers.StandardizeString(rightConstant.Value, true);
                                    var sb = new StringBuilder(str.Length * count);
                                    for (var i = 0; i < count; i++)
                                    {
                                        sb.Append(str);
                                    }

                                    return new ConstantValueStatement(DataTypes.String,
                                        sb.ToString(),
                                        arithmeticEvaluationStatement.Info
                                    )
                                    {
                                        ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                    };
                                }

                                if (leftConstant.IsString() || rightConstant.IsDecimal())
                                {
                                    if (!int.TryParse(rightConstant.Value, out var count))
                                    {
                                        throw BashTranspilerHelpers.InvalidStatementStructure(scope, rightConstant);
                                    }

                                    var str = BashTranspilerHelpers.StandardizeString(leftConstant.Value, true);
                                    var sb = new StringBuilder(str.Length * count);
                                    for (var i = 0; i < count; i++)
                                    {
                                        sb.Append(str);
                                    }

                                    return new ConstantValueStatement(DataTypes.String,
                                        sb.ToString(),
                                        arithmeticEvaluationStatement.Info
                                    )
                                    {
                                        ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                    };
                                }

                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    arithmeticEvaluationStatement);
                            }

                            var result = new ArithmeticEvaluationStatement(
                                left,
                                arithmeticEvaluationStatement.Operator,
                                right,
                                arithmeticEvaluationStatement.Info)
                            {
                                ParentStatement = arithmeticEvaluationStatement.ParentStatement
                            };

                            left.ParentStatement = result;
                            right.ParentStatement = result;

                            return result;
                        }

                        default:
                            throw new InvalidOperationException();
                    }
                }

                case AssignmentStatement assignmentStatement:
                {
                    if (!(assignmentStatement.LeftSide is VariableAccessStatement variableAccessStatement))
                    {
                        throw new InvalidStatementStructureCompilerException(assignmentStatement,
                            assignmentStatement.Info);
                    }

                    if (!scope.TryGetVariableInfo(variableAccessStatement, out var varInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }

                    var right = ProcessEvaluation(context, scope, assignmentStatement.RightSide);

                    var result = new AssignmentStatement(variableAccessStatement, right, assignmentStatement.Info);

                    variableAccessStatement.ParentStatement = result;
                    right.ParentStatement = result;

                    return result;
                }

                case FunctionCallStatement functionCallStatement:
                {
                    if (scope.TryGetFunctionInfo(functionCallStatement, out var functionInfo))
                    {
                        if (functionInfo.InlinedStatement != null)
                        {
                            if (functionInfo.InlinedStatement is EvaluationStatement evaluationStatement)
                            {
                                return ProcessEvaluation(context, scope, evaluationStatement);
                            }

                            if (functionInfo.InlinedStatement is FunctionCallStatement funcCallStt)
                            {
                                return ProcessEvaluation(context, scope, funcCallStt);
                            }
                        }

                        if (functionCallStatement.Parameters != null && functionCallStatement.Parameters.Length > 0)
                        {
                            var parameters = new List<EvaluationStatement>(functionCallStatement.Parameters.Length);
                            foreach (var param in functionCallStatement.Parameters)
                            {
                                parameters.Add(ProcessEvaluation(context, scope, param));
                            }

                            var paramArray = parameters.ToArray();

                            var result = new FunctionCallStatement(functionCallStatement.ObjectName,
                                functionCallStatement.FunctionName, functionCallStatement.DataType,
                                paramArray, functionCallStatement.Info)
                            {
                                ParentStatement = functionCallStatement.ParentStatement
                            };

                            foreach (var param in paramArray)
                            {
                                param.ParentStatement = result;
                            }

                            return result;
                        }

                        return functionCallStatement;
                    }

                    throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                        functionCallStatement.Info);
                }

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}