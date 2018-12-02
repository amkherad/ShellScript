using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations
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

        public abstract PinnedVariableResult PinEvaluationToVariable(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, EvaluationStatement statement);

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

        public abstract ExpressionResult CallApiFunction<TApiFunc>(ExpressionBuilderParams p,
            EvaluationStatement[] parameters,
            IStatement parentStatement, StatementInfo statementInfo) where TApiFunc : IApiFunc;


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
                        if (constantValueStatement.IsInteger())
                        {
                            if (long.TryParse(constantValueStatement.Value, out _))
                            {
                                return constantValueStatement;
                            }
                        }
                        else if (long.TryParse(constantValueStatement.Value, out _))
                        {
                            return new ConstantValueStatement(TypeDescriptor.Integer, constantValueStatement.Value,
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

                        return new ConstantValueStatement(TypeDescriptor.String, str, constantValueStatement.Info);
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
                        return new ConstantValueStatement(constInfo.TypeDescriptor, constInfo.Value,
                            variableAccessStatement.Info);
                    }

                    if (scope.TryGetPrototypeInfo(variableAccessStatement, out _) ||
                        scope.TryGetFunctionInfo(variableAccessStatement, out _))
                    {
                        return variableAccessStatement;
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
                                if (constantValueStatement.TypeDescriptor.IsInteger())
                                {
                                    if (!long.TryParse(constantValueStatement.Value, out var value))
                                    {
                                        throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                            bitwiseEvaluationStatement);
                                    }

                                    return new ConstantValueStatement(
                                        constantValueStatement.TypeDescriptor,
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
                                if (leftConstantValue.IsInteger() &&
                                    long.TryParse(leftConstantValue.Value, out var leftInteger))
                                {
                                    if (rightConstantValue.IsInteger() &&
                                        long.TryParse(rightConstantValue.Value, out var rightInteger))
                                    {
                                        long resultVal;
                                        switch (bitwiseEvaluationStatement.Operator)
                                        {
                                            case BitwiseAndOperator _:
                                                resultVal = leftInteger & rightInteger;
                                                break;
                                            case BitwiseOrOperator _:
                                                resultVal = leftInteger | rightInteger;
                                                break;
                                            case XorOperator _:
                                                resultVal = leftInteger ^ rightInteger;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(TypeDescriptor.Integer,
                                            resultVal.ToString(CultureInfo.InvariantCulture),
                                            bitwiseEvaluationStatement.Info)
                                        {
                                            ParentStatement = bitwiseEvaluationStatement.ParentStatement
                                        };
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        bitwiseEvaluationStatement.Operator.GetType(),
                                        rightConstantValue.TypeDescriptor,
                                        rightConstantValue.Info);
                                }

                                if (leftConstantValue.TypeDescriptor.IsBoolean() &&
                                    rightConstantValue.TypeDescriptor.IsBoolean() &&
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

                                    return new ConstantValueStatement(TypeDescriptor.Boolean,
                                        resultVal.ToString(CultureInfo.InvariantCulture),
                                        bitwiseEvaluationStatement.Info)
                                    {
                                        ParentStatement = bitwiseEvaluationStatement.ParentStatement
                                    };
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    bitwiseEvaluationStatement.Operator.GetType(), leftConstantValue.TypeDescriptor,
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
                                         leftConstantValue.IsInteger()*/) &&
                                        StatementHelpers.TryParseBooleanFromString(leftConstantValue.Value,
                                            out var leftBool))
                                    {
                                        if ((rightConstantValue.IsBoolean() /*||
                                             rightConstantValue.IsInteger()*/) &&
                                            StatementHelpers.TryParseBooleanFromString(rightConstantValue.Value,
                                                out var rightBool))
                                        {
                                            return new ConstantValueStatement(TypeDescriptor.Boolean,
                                                (logicalEvaluationStatement.Operator is LogicalAndOperator
                                                    ? leftBool && rightBool
                                                    : leftBool || rightBool).ToString(CultureInfo.InvariantCulture),
                                                logicalEvaluationStatement.Info)
                                            {
                                                ParentStatement = logicalEvaluationStatement.ParentStatement
                                            };
                                        }

                                        throw new InvalidOperatorForTypeCompilerException(
                                            logicalEvaluationStatement.Operator.GetType(),
                                            rightConstantValue.TypeDescriptor,
                                            rightConstantValue.Info);
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        logicalEvaluationStatement.Operator.GetType(), leftConstantValue.TypeDescriptor,
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
                                    if (leftConstant.IsInteger() && rightConstant.IsInteger() &&
                                        long.TryParse(leftConstant.Value, out var leftInteger) &&
                                        long.TryParse(rightConstant.Value, out var rightInteger)
                                    )
                                    {
                                        return new ConstantValueStatement(
                                            TypeDescriptor.Boolean,
                                            (logicalEvaluationStatement.Operator is EqualOperator
                                                ? leftInteger == rightInteger
                                                : leftInteger != rightInteger).ToString(CultureInfo.InvariantCulture),
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
                                            TypeDescriptor.Boolean,
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
                                    TypeDescriptor.Boolean,
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
                                    if (leftConstantValue.IsInteger() && rightConstantValue.IsInteger() &&
                                        long.TryParse(leftConstantValue.Value, out var leftInteger) &&
                                        long.TryParse(rightConstantValue.Value, out var rightInteger))
                                    {
                                        bool resultVal;
                                        switch (logicalEvaluationStatement.Operator)
                                        {
                                            case GreaterOperator _:
                                                resultVal = leftInteger > rightInteger;
                                                break;
                                            case GreaterEqualOperator _:
                                                resultVal = leftInteger >= rightInteger;
                                                break;
                                            case LessOperator _:
                                                resultVal = leftInteger < rightInteger;
                                                break;
                                            case LessEqualOperator _:
                                                resultVal = leftInteger <= rightInteger;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(TypeDescriptor.Boolean,
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

                                        return new ConstantValueStatement(TypeDescriptor.Boolean,
                                            resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                            logicalEvaluationStatement.Info)
                                        {
                                            ParentStatement = logicalEvaluationStatement.ParentStatement
                                        };
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        logicalEvaluationStatement.Operator.GetType(),
                                        rightConstantValue.TypeDescriptor,
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

                                    return new ConstantValueStatement(TypeDescriptor.Boolean,
                                        resultVal.ToString(NumberFormatInfo.InvariantInfo),
                                        logicalEvaluationStatement.Info)
                                    {
                                        ParentStatement = logicalEvaluationStatement.ParentStatement
                                    };
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    logicalEvaluationStatement.Operator.GetType(), leftConstantValue.TypeDescriptor,
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
                                if (constantValueStatement.TypeDescriptor.IsBoolean() ||
                                    constantValueStatement.IsNumber())
                                {
                                    if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value,
                                        out var value))
                                    {
                                        throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                            logicalEvaluationStatement);
                                    }

                                    return new ConstantValueStatement(
                                        constantValueStatement.TypeDescriptor,
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
                                            constantValueStatement.TypeDescriptor,
                                            (-longResult).ToString(NumberFormatInfo.InvariantInfo),
                                            constantValueStatement.Info
                                        );
                                    }

                                    if (double.TryParse(constantValueStatement.Value, out var doubleResult))
                                    {
                                        return new ConstantValueStatement(
                                            constantValueStatement.TypeDescriptor,
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
                                    if (leftConstant.IsInteger() && rightConstant.IsInteger() &&
                                        long.TryParse(leftConstant.Value, out var leftInteger) &&
                                        long.TryParse(rightConstant.Value, out var rightInteger))
                                    {
                                        long resultVal;
                                        switch (arithmeticEvaluationStatement.Operator)
                                        {
                                            case AdditionOperator _:
                                                resultVal = leftInteger + rightInteger;
                                                break;
                                            case SubtractionOperator _:
                                                resultVal = leftInteger - rightInteger;
                                                break;
                                            case MultiplicationOperator _:
                                                resultVal = leftInteger * rightInteger;
                                                break;
                                            case DivisionOperator _:
                                                resultVal = leftInteger / rightInteger;
                                                break;
                                            case ModulusOperator _:
                                                resultVal = leftInteger % rightInteger;
                                                break;
                                            //case ReminderOperator _:
                                            //    resultVal = leftInteger % rightInteger;
                                            //    break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(TypeDescriptor.Integer,
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

                                        return new ConstantValueStatement(TypeDescriptor.Integer,
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
                                    return new ConstantValueStatement(TypeDescriptor.String,
                                        BashTranspilerHelpers.StandardizeString(leftConstant.Value, true) +
                                        BashTranspilerHelpers.StandardizeString(rightConstant.Value, true),
                                        arithmeticEvaluationStatement.Info
                                    )
                                    {
                                        ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                    };
                                }

                                if (leftConstant.IsInteger() || rightConstant.IsString())
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

                                    return new ConstantValueStatement(TypeDescriptor.String,
                                        sb.ToString(),
                                        arithmeticEvaluationStatement.Info
                                    )
                                    {
                                        ParentStatement = arithmeticEvaluationStatement.ParentStatement
                                    };
                                }

                                if (leftConstant.IsString() || rightConstant.IsInteger())
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

                                    return new ConstantValueStatement(TypeDescriptor.String,
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

                case TypeCastStatement typeCastStatement:
                {
                    var right = ProcessEvaluation(context, scope, typeCastStatement.Target);

                    var result = new TypeCastStatement(typeCastStatement.TypeDescriptor, right, typeCastStatement.Info)
                    {
                        ParentStatement = typeCastStatement.ParentStatement
                    };

                    right.ParentStatement = result;

                    return result;
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

                    var result = new AssignmentStatement(variableAccessStatement, right, assignmentStatement.Info)
                    {
                        ParentStatement = assignmentStatement.ParentStatement
                    };

                    variableAccessStatement.ParentStatement = result;
                    right.ParentStatement = result;

                    return result;
                }

                case IndexerAccessStatement indexerAccessStatement:
                {
                    if (!(indexerAccessStatement.Source is VariableAccessStatement variableAccessStatement))
                    {
                        throw new InvalidStatementStructureCompilerException(indexerAccessStatement,
                            indexerAccessStatement.Info);
                    }

                    if (!scope.TryGetVariableInfo(variableAccessStatement, out _))
                    {
                        throw new IdentifierNotFoundCompilerException(variableAccessStatement);
                    }

                    var indexer = ProcessEvaluation(context, scope, indexerAccessStatement.Indexer);

                    var result =
                        new IndexerAccessStatement(variableAccessStatement, indexer, indexerAccessStatement.Info)
                        {
                            ParentStatement = indexerAccessStatement.ParentStatement
                        };

                    variableAccessStatement.ParentStatement = result;
                    indexer.ParentStatement = result;

                    return result;
                }

                case ArrayStatement arrayStatement:
                {
                    if (arrayStatement.Elements == null)
                    {
                        return new ArrayStatement(arrayStatement.Type, arrayStatement.Length, null,
                            arrayStatement.Info);
                    }

                    var elementType = arrayStatement.Type.DataType ^ DataTypes.Array;

                    var items = new List<EvaluationStatement>();

                    foreach (var element in arrayStatement.Elements)
                    {
                        var elem = ProcessEvaluation(context, scope, element);

                        var elemType = elem.GetDataType(context, scope);
                        
                        if (!StatementHelpers.IsAssignableFrom(context, scope, elementType, elemType))
                        {
                            throw new TypeMismatchCompilerException(elemType, elementType, elem.Info);
                        }

                        items.Add(elem);
                    }

                    var itemsArray = items.ToArray();

                    var result = new ArrayStatement(arrayStatement.Type, null, itemsArray, arrayStatement.Info)
                    {
                        ParentStatement = arrayStatement.ParentStatement
                    };
                    foreach (var element in itemsArray)
                    {
                        element.ParentStatement = result;
                    }

                    return result;
                }

                case FunctionCallStatement functionCallStatement:
                {
                    var funcInfo =
                        FunctionStatementTranspilerBase.GetFunctionInfoFromFunctionCall(context, scope,
                            functionCallStatement, out var sourceObjectInfo);

                    if (!FunctionStatementTranspilerBase.ValidateFunctionCall(context, scope, funcInfo,
                        functionCallStatement, out var exception))
                    {
//                        throw new InvalidStatementStructureCompilerException(functionCallStatement.Fqn,
//                            functionCallStatement.Info);
                        throw exception;
                    }

//                  if (functionInfo.InlinedStatement != null)
//                  {
//                      if (functionInfo.InlinedStatement is EvaluationStatement evaluationStatement)
//                      {
//                          return ProcessEvaluation(context, scope, evaluationStatement);
//                      }
//
//                      if (functionInfo.InlinedStatement is FunctionCallStatement funcCallStt)
//                      {
//                          return ProcessEvaluation(context, scope, funcCallStt);
//                      }
//                  }

                    if (functionCallStatement.Parameters != null && functionCallStatement.Parameters.Length > 0)
                    {
                        var parameters = new List<EvaluationStatement>(functionCallStatement.Parameters.Length);
                        foreach (var param in functionCallStatement.Parameters)
                        {
                            parameters.Add(ProcessEvaluation(context, scope, param));
                        }

                        var paramArray = parameters.ToArray();

                        var result = new FunctionCallStatement(functionCallStatement.ClassName,
                            functionCallStatement.FunctionName, functionCallStatement.TypeDescriptor,
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

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}