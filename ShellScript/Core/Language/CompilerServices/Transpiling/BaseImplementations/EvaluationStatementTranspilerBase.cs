using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class EvaluationStatementTranspilerBase : StatementTranspilerBase, IPlatformEvaluationStatementTranspiler
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

            VariableAccessStatement variable = null;

            //check for all variables defined.
            if (evalStt.TreeAny(stt =>
            {
                if (stt is VariableAccessStatement varAccessStt)
                {
                    variable = varAccessStt;
                    return !scope.IsIdentifierExists(varAccessStt.VariableName);
                }

                return false;
            }))
            {
                message = IdentifierNotFoundCompilerException.CreateMessage(variable.VariableName, variable.Info);
                return false;
            }

            return base.Validate(context, scope, statement, out message);
        }

        public abstract string PinEvaluationToVariable(Context context, Scope scope, TextWriter metaWriter,
            TextWriter pinCodeWriter, EvaluationStatement statement);

        public abstract (DataTypes, string) GetInline(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement);

        public abstract (DataTypes, string) GetInlineConditional(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement);


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
                        if (constantValueStatement.IsDecimal() &&
                            long.TryParse(constantValueStatement.Value, out _))
                        {
                            return constantValueStatement;
                        }
                        
                        if (double.TryParse(constantValueStatement.Value, out _))
                        {
                            return constantValueStatement;
                        }
                        
                        throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                            constantValueStatement);
                    }
                    
                    return constantValueStatement;
                }

                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement.VariableName, out _))
                    {
                        return variableAccessStatement;
                    }

                    if (scope.TryGetConstantInfo(variableAccessStatement.VariableName, out var constInfo))
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
                                    );
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
                                        long result;
                                        switch (bitwiseEvaluationStatement.Operator)
                                        {
                                            case BitwiseAndOperator _:
                                                result = leftDecimal & rightDecimal;
                                                break;
                                            case BitwiseOrOperator _:
                                                result = leftDecimal | rightDecimal;
                                                break;
                                            case XorOperator _:
                                                result = leftDecimal ^ rightDecimal;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Decimal,
                                            result.ToString(CultureInfo.InvariantCulture),
                                            bitwiseEvaluationStatement.Info);
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
                                    bool result;
                                    switch (bitwiseEvaluationStatement.Operator)
                                    {
                                        case BitwiseAndOperator _:
                                            result = leftBool && rightBool;
                                            break;
                                        case BitwiseOrOperator _:
                                            result = leftBool || rightBool;
                                            break;
                                        case XorOperator _:
                                            result = leftBool ^ rightBool;
                                            break;
                                        default:
                                            throw new InvalidOperationException();
                                    }

                                    return new ConstantValueStatement(DataTypes.Boolean,
                                        result.ToString(CultureInfo.InvariantCulture),
                                        bitwiseEvaluationStatement.Info);
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    bitwiseEvaluationStatement.Operator.GetType(), leftConstantValue.DataType,
                                    leftConstantValue.Info);
                            }

                            return new BitwiseEvaluationStatement(
                                left,
                                bitwiseEvaluationStatement.Operator,
                                right,
                                bitwiseEvaluationStatement.Info);
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

                            if (left is ConstantValueStatement leftConstantValue &&
                                right is ConstantValueStatement rightConstantValue)
                            {
                                if ((leftConstantValue.DataType == DataTypes.Boolean ||
                                     leftConstantValue.IsDecimal()) &&
                                    StatementHelpers.TryParseBooleanFromString(leftConstantValue.Value,
                                        out var leftBool))
                                {
                                    if ((leftConstantValue.DataType == DataTypes.Boolean ||
                                         rightConstantValue.IsDecimal()) &&
                                        StatementHelpers.TryParseBooleanFromString(rightConstantValue.Value,
                                            out var rightBool))
                                    {
                                        return new ConstantValueStatement(DataTypes.Boolean,
                                            (logicalEvaluationStatement.Operator is LogicalAndOperator
                                                ? leftBool && rightBool
                                                : leftBool || rightBool).ToString(CultureInfo.InvariantCulture),
                                            logicalEvaluationStatement.Info);
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        logicalEvaluationStatement.Operator.GetType(), rightConstantValue.DataType,
                                        rightConstantValue.Info);
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    logicalEvaluationStatement.Operator.GetType(), leftConstantValue.DataType,
                                    leftConstantValue.Info);
                            }

                            return new LogicalEvaluationStatement(
                                left,
                                logicalEvaluationStatement.Operator,
                                right,
                                logicalEvaluationStatement.Info);
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
                                        );
                                    }

                                    if (double.TryParse(leftConstant.Value, out var leftFloat) &&
                                        double.TryParse(rightConstant.Value, out var rightFloat))
                                    {
                                        return new ConstantValueStatement(
                                            DataTypes.Boolean,
                                            (Math.Abs(leftFloat - rightFloat) < double.Epsilon).ToString(CultureInfo
                                                .InvariantCulture),
                                            logicalEvaluationStatement.Info
                                        );
                                    }
                                }

                                return new ConstantValueStatement(
                                    DataTypes.Boolean,
                                    (logicalEvaluationStatement.Operator is EqualOperator
                                        ? leftConstant.Value == rightConstant.Value
                                        : leftConstant.Value != rightConstant.Value)
                                    .ToString(CultureInfo.InvariantCulture),
                                    logicalEvaluationStatement.Info
                                );
                            }

                            return new LogicalEvaluationStatement(
                                left,
                                logicalEvaluationStatement.Operator,
                                right,
                                logicalEvaluationStatement.Info);
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
                                        bool result;
                                        switch (logicalEvaluationStatement.Operator)
                                        {
                                            case GreaterOperator _:
                                                result = leftDecimal > rightDecimal;
                                                break;
                                            case GreaterEqualOperator _:
                                                result = leftDecimal >= rightDecimal;
                                                break;
                                            case LessOperator _:
                                                result = leftDecimal < rightDecimal;
                                                break;
                                            case LessEqualOperator _:
                                                result = leftDecimal <= rightDecimal;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Boolean,
                                            result.ToString(NumberFormatInfo.InvariantInfo),
                                            logicalEvaluationStatement.Info);
                                    }

                                    if (double.TryParse(leftConstantValue.Value, out var leftFloat) &&
                                        double.TryParse(rightConstantValue.Value, out var rightFloat))
                                    {
                                        bool result;
                                        switch (logicalEvaluationStatement.Operator)
                                        {
                                            case GreaterOperator _:
                                                result = leftFloat > rightFloat;
                                                break;
                                            case GreaterEqualOperator _:
                                                result = leftFloat >= rightFloat;
                                                break;
                                            case LessOperator _:
                                                result = leftFloat < rightFloat;
                                                break;
                                            case LessEqualOperator _:
                                                result = leftFloat <= rightFloat;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Boolean,
                                            result.ToString(NumberFormatInfo.InvariantInfo),
                                            logicalEvaluationStatement.Info);
                                    }

                                    throw new InvalidOperatorForTypeCompilerException(
                                        logicalEvaluationStatement.Operator.GetType(), rightConstantValue.DataType,
                                        rightConstantValue.Info);
                                }

                                if (leftConstantValue.IsString() && rightConstantValue.IsString())
                                {
                                    bool result;
                                    var comp = context.StringComparer.Compare(leftConstantValue.Value,
                                        rightConstantValue.Value);

                                    switch (logicalEvaluationStatement.Operator)
                                    {
                                        case GreaterOperator _:
                                            result = comp > 0;
                                            break;
                                        case GreaterEqualOperator _:
                                            result = comp >= 0;
                                            break;
                                        case LessOperator _:
                                            result = comp < 0;
                                            break;
                                        case LessEqualOperator _:
                                            result = comp <= 0;
                                            break;
                                        default:
                                            throw new InvalidOperationException();
                                    }

                                    return new ConstantValueStatement(DataTypes.Boolean,
                                        result.ToString(NumberFormatInfo.InvariantInfo),
                                        logicalEvaluationStatement.Info);
                                }

                                throw new InvalidOperatorForTypeCompilerException(
                                    logicalEvaluationStatement.Operator.GetType(), leftConstantValue.DataType,
                                    leftConstantValue.Info);
                            }

                            return new LogicalEvaluationStatement(
                                left,
                                logicalEvaluationStatement.Operator,
                                right,
                                logicalEvaluationStatement.Info);
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
                                    );
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
                                    if (scope.TryGetVariableInfo(variableAccessStatement.VariableName, out _))
                                    {
                                        return variableAccessStatement;
                                    }

                                    if (scope.TryGetConstantInfo(variableAccessStatement.VariableName, out _))
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
                                        );
                                    }
                                }

                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    arithmeticEvaluationStatement);
                            }

                            return new ArithmeticEvaluationStatement(
                                null,
                                arithmeticEvaluationStatement.Operator,
                                right,
                                arithmeticEvaluationStatement.Info);
                        }
                        case AdditionOperator _:
                        case SubtractionOperator _:
                        case MultiplicationOperator _:
                        case DivisionOperator _:
                        case ModulusOperator _:
                        case ReminderOperator _:
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
                                        long result;
                                        switch (arithmeticEvaluationStatement.Operator)
                                        {
                                            case AdditionOperator _:
                                                result = leftDecimal + rightDecimal;
                                                break;
                                            case SubtractionOperator _:
                                                result = leftDecimal - rightDecimal;
                                                break;
                                            case MultiplicationOperator _:
                                                result = leftDecimal * rightDecimal;
                                                break;
                                            case DivisionOperator _:
                                                result = leftDecimal / rightDecimal;
                                                break;
                                            case ModulusOperator _:
                                                result = leftDecimal / rightDecimal;
                                                break;
                                            case ReminderOperator _:
                                                result = leftDecimal % rightDecimal;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Decimal,
                                            result.ToString(NumberFormatInfo.InvariantInfo),
                                            arithmeticEvaluationStatement.Info
                                        );
                                    }

                                    if (double.TryParse(leftConstant.Value, out var leftFloat) &&
                                        double.TryParse(rightConstant.Value, out var rightFloat))
                                    {
                                        double result;
                                        switch (arithmeticEvaluationStatement.Operator)
                                        {
                                            case AdditionOperator _:
                                                result = leftFloat + rightFloat;
                                                break;
                                            case SubtractionOperator _:
                                                result = leftFloat - rightFloat;
                                                break;
                                            case MultiplicationOperator _:
                                                result = leftFloat * rightFloat;
                                                break;
                                            case DivisionOperator _:
                                                result = leftFloat / rightFloat;
                                                break;
                                            case ModulusOperator _:
                                                result = leftFloat / rightFloat;
                                                break;
                                            case ReminderOperator _:
                                                result = leftFloat % rightFloat;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }

                                        return new ConstantValueStatement(DataTypes.Decimal,
                                            result.ToString(NumberFormatInfo.InvariantInfo),
                                            arithmeticEvaluationStatement.Info
                                        );
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
                                    );
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
                                    );
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
                                    );
                                }
                                
                                throw BashTranspilerHelpers.InvalidStatementStructure(scope,
                                    arithmeticEvaluationStatement);
                            }

                            return new ArithmeticEvaluationStatement(
                                left,
                                arithmeticEvaluationStatement.Operator,
                                right,
                                arithmeticEvaluationStatement.Info);
                        }
                        
                        default:
                            throw new InvalidOperationException();
                    }
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
                        
                        return functionCallStatement;
                    }

                    if (string.IsNullOrWhiteSpace(functionCallStatement.ObjectName))
                    {
                        
                    }
                    else if (context.Platform.Api.TryGetClass(functionCallStatement.ObjectName, out var sdkClass))
                    {
                        if (sdkClass.TryGetFunction(functionCallStatement.FunctionName, out var sdkFunc))
                        {
                            //sdkFunc.
                        }
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