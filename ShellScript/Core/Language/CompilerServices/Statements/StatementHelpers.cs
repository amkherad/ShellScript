using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public static class StatementHelpers
    {
        public static readonly IStatement[] EmptyStatements = new IStatement[0];
        
        public static IStatement[] CreateChildren(params IStatement[] children)
        {
            var result = new List<IStatement>(children.Length);
            foreach (var child in children)
            {
                if (child != null)
                    result.Add(child);
            }

            return result.ToArray();
        }

        public static void TreeForEach<TStatement>(this IStatement statement, Action<TStatement> action)
            where TStatement : IStatement
        {
            foreach (var child in statement.TraversableChildren)
            {
                if (child is TStatement castChild)
                {
                    action(castChild);
                }

                TreeForEach<TStatement>(child, action);
            }
        }


        public static bool TreeContains<TStatement>(this IStatement statement)
            where TStatement : IStatement
        {
            foreach (var child in statement.TraversableChildren)
            {
                if (child is TStatement)
                {
                    return true;
                }

                if (TreeContains<TStatement>(child))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TreeContains(this IStatement statement, params Type[] types)
        {
            foreach (var child in statement.TraversableChildren)
            {
                var childType = child.GetType();
                foreach (var type in types)
                {
                    if (type.IsAssignableFrom(childType))
                    {
                        return true;
                    }
                }

                if (TreeContains(child, types))
                {
                    return true;
                }
            }

            return false;
        }


        public static bool TreeAny(this IStatement statement, Predicate<IStatement> predicate)
        {
            foreach (var child in statement.TraversableChildren)
            {
                if (predicate(child))
                {
                    return true;
                }

                if (TreeAny(child, predicate))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TreeAll(this IStatement statement, Predicate<IStatement> predicate)
        {
            var atLeastOneExecuted = false;
            foreach (var child in statement.TraversableChildren)
            {
                if (predicate(child))
                {
                    return false;
                }

                if (TreeAll(child, predicate))
                {
                    return false;
                }

                atLeastOneExecuted = true;
            }

            return atLeastOneExecuted;
        }


        public class ExpressionTypes
        {
            public List<DataTypes> Types { get; set; }
            public bool ContainsFunctionCall { get; set; }
            public List<VariableAccessStatement> NonExistenceVariables { get; set; }

            public HashSet<Type> OperatorTypes { get; set; }
        }

        public static ExpressionTypes TraverseTreeAndGetExpressionTypes(Context context, Scope scope,
            IStatement statement)
        {
            var result = new ExpressionTypes();
            var types = new List<DataTypes>();
            var nonExistenceVariables = new List<VariableAccessStatement>();
            var operators = new HashSet<Type>();

            result.Types = types;
            result.NonExistenceVariables = nonExistenceVariables;
            result.OperatorTypes = operators;

            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    types.Add(constantValueStatement.DataType);
                    break;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement, out VariableInfo varInfo))
                    {
                        types.Add(varInfo.DataType);
                    }
                    else
                    {
                        nonExistenceVariables.Add(variableAccessStatement);
                    }

                    break;
                }
                case IOperator op:
                {
                    operators.Add(op.GetType());
                    break;
                }
                case FunctionCallStatement functionCallStatement:
                {
                    types.Add(functionCallStatement.DataType);
                    break;
                }
                default:
                {
                    foreach (var child in statement.TraversableChildren)
                    {
                        if (child is IOperator)
                        {
                            operators.Add(child.GetType());
                            continue;
                        }

                        var childResult = TraverseTreeAndGetExpressionTypes(context, scope, child);

                        foreach (var t in childResult.Types)
                            types.Add(t);

                        foreach (var nev in childResult.NonExistenceVariables)
                            nonExistenceVariables.Add(nev);

                        foreach (var opType in childResult.OperatorTypes)
                            operators.Add(opType);

                        result.ContainsFunctionCall = result.ContainsFunctionCall || childResult.ContainsFunctionCall;
                    }

                    break;
                }
            }

            return result;
        }

        public static DataTypes OperateDataTypes(IOperator op, DataTypes a, DataTypes b)
        {
            if (a == b)
            {
                return a;
            }

            if (a.IsDecimal())
            {
                if (b.IsDecimal())
                {
                    return DataTypes.Decimal;
                }

                if (b.IsNumericOrFloat())
                {
                    return DataTypes.Numeric;
                }
            }

            if (a.IsNumber())
            {
                if (b.IsNumber())
                {
                    return DataTypes.Numeric;
                }

                if (b == DataTypes.String && op is AdditionOperator)
                {
                    return DataTypes.String;
                }

                throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
            }

            if (a == DataTypes.String)
            {
                if (b == DataTypes.Decimal || b == DataTypes.Float || b == DataTypes.Numeric)
                {
                    return DataTypes.String;
                }

                throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
            }

            throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
        }

        public static DataTypes GetDataType(this EvaluationStatement evaluationStatement, Context context, Scope scope)
        {
            switch (evaluationStatement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    return constantValueStatement.DataType;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (!scope.TryGetVariableInfo(variableAccessStatement, out var variableInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                            variableAccessStatement.Info);
                    }

                    return variableInfo.DataType;
                }
                case FunctionCallStatement functionCallStatement:
                {
                    if (!scope.TryGetFunctionInfo(functionCallStatement, out var functionInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                            functionCallStatement.Info);
                    }

                    return functionInfo.DataType;
                }
                case BitwiseEvaluationStatement bitwiseEvaluationStatement:
                {
                    switch (bitwiseEvaluationStatement.Operator)
                    {
                        case BitwiseNotOperator _:
                        {
                            return bitwiseEvaluationStatement.Right.GetDataType(context, scope);
                        }
                        case BitwiseAndOperator _:
                        case BitwiseOrOperator _:
                        case XorOperator _:
                        {
                            return OperateDataTypes(
                                bitwiseEvaluationStatement.Operator,
                                bitwiseEvaluationStatement.Left.GetDataType(context, scope),
                                bitwiseEvaluationStatement.Right.GetDataType(context, scope)
                            );
                        }
                    }

                    return DataTypes.Numeric;
                }
                case LogicalEvaluationStatement _: //logicalEvaluationStatement
                {
                    return DataTypes.Boolean;
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case IncrementOperator _:
                        case DecrementOperator _:
                        case NegativeNumberOperator _:
                        {
                            return (arithmeticEvaluationStatement.Right ?? arithmeticEvaluationStatement.Left)
                                .GetDataType(context, scope);
                        }
                        case AdditionOperator _:
                        case SubtractionOperator _:
                        case MultiplicationOperator _:
                        case DivisionOperator _:
                        //case ReminderOperator _:
                        case ModulusOperator _:
                        {
                            return OperateDataTypes(
                                arithmeticEvaluationStatement.Operator,
                                arithmeticEvaluationStatement.Left.GetDataType(context, scope),
                                arithmeticEvaluationStatement.Right.GetDataType(context, scope)
                            );
                        }
                    }

                    return DataTypes.Numeric;
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public static bool IsAssignableFrom(DataTypes destination, DataTypes source)
        {
            if (destination == source)
            {
                return true;
            }

            if (destination == DataTypes.Numeric)
            {
                if (source.IsNumber())
                {
                    return true;
                }
            }
            else if (destination == DataTypes.Float)
            {
                if (source == DataTypes.Decimal)
                {
                    return true;
                }
            }

            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UnEscapeString(string str)
        {
            if (str[0] == '"' && str[str.Length - 1] == '"')
            {
                str = str.Substring(1, str.Length - 2);
            }
            
            if (str.Contains("\\\""))
            {
                str = str.Replace("\\\"", "\"");
            }
            
            if (str.Contains("\\\'"))
            {
                str = str.Replace("\\\'", "\'");
            }
            
            
            if (str.Contains("\\r"))
            {
                str = str.Replace("\\r", "\r");
            }

            if (str.Contains("\\n"))
            {
                str = str.Replace("\\n", "\n");
            }

            if (str.Contains("\\\\"))
            {
                str = str.Replace("\\\\", "\\");
            }
            

            return str;
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this DataTypes dataType)
        {
            return dataType == DataTypes.Decimal || dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Decimal || dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumericOrFloat(this DataTypes dataType)
        {
            return dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumericOrFloat(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloat(this DataTypes dataType)
        {
            return dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloat(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDecimal(this DataTypes dataType)
        {
            return dataType == DataTypes.Decimal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDecimal(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.Decimal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this DataTypes dataType)
        {
            return dataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this DataTypes dataType)
        {
            return dataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this DataTypes dataType)
        {
            return dataType == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this DataTypes dataType)
        {
            return dataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFunction(this DataTypes dataType)
        {
            return dataType == DataTypes.Delegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFunction(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.DataType == DataTypes.Delegate;
        }

        public static bool IsAbsoluteValue(IStatement statement, out bool isTrue)
        {
            if (statement is ConstantValueStatement constantValueStatement)
            {
                return TryParseBooleanFromString(constantValueStatement.Value, out isTrue);
            }

            isTrue = false;
            return false;
        }

        public static bool IsAbsoluteBooleanValue(IStatement statement, out bool isTrue)
        {
            if (statement is ConstantValueStatement constantValueStatement &&
                constantValueStatement.IsBoolean())
            {
                return TryParseBooleanFromString(constantValueStatement.Value, out isTrue);
            }

            isTrue = false;
            return false;
        }

        public static bool TryParseBooleanFromString(string value, out bool result)
        {
            if (bool.TryParse(value, out var boolResult))
            {
                result = boolResult;
                return true;
            }

            if (long.TryParse(value, out var intResult))
            {
                result = intResult != 0;
                return true;
            }

            if (double.TryParse(value, out var doubleResult))
            {
                result = intResult != 0;
                return true;
            }

            result = default;
            return false;
        }
    }
}