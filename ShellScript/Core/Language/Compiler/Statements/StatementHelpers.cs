using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public static class StatementHelpers
    {
        public static readonly IStatement[] EmptyStatements = new IStatement[0];

        public static IStatement[] CreateChildren(params IStatement[] children)
        {
            if (children == null)
            {
                return EmptyStatements;
            }

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

        public static int TreeCount(this IStatement statement, Predicate<IStatement> predicate)
        {
            var count = 0;
            foreach (var child in statement.TraversableChildren)
            {
                if (predicate(child))
                {
                    count += 1;
                }

                count += TreeCount(child, predicate);
            }

            return count;
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
            public List<TypeDescriptor> Types { get; set; }
            public bool ContainsFunctionCall { get; set; }
            public List<VariableAccessStatement> NonExistenceVariables { get; set; }

            public HashSet<Type> OperatorTypes { get; set; }
        }

        public static ExpressionTypes TraverseTreeAndGetExpressionTypes(Context context, Scope scope,
            IStatement statement)
        {
            var result = new ExpressionTypes();
            var types = new List<TypeDescriptor>();
            var nonExistenceVariables = new List<VariableAccessStatement>();
            var operators = new HashSet<Type>();

            result.Types = types;
            result.NonExistenceVariables = nonExistenceVariables;
            result.OperatorTypes = operators;

            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    types.Add(constantValueStatement.TypeDescriptor);
                    break;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement, out VariableInfo varInfo))
                    {
                        types.Add(varInfo.TypeDescriptor);
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
                    types.Add(functionCallStatement.TypeDescriptor);
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

        public static TypeDescriptor OperateDataTypes(IOperator op, TypeDescriptor a, TypeDescriptor b)
        {
            if (a == b)
            {
                var dataType = a.DataType;
                if (dataType.IsDelegate() || dataType.IsObject() || dataType.IsArray())
                {
                    throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
                }

                return a;
            }

            if (a.IsInteger())
            {
                if (b.IsInteger())
                {
                    return TypeDescriptor.Integer;
                }

                if (b.IsNumericOrFloat())
                {
                    return TypeDescriptor.Numeric;
                }
            }

            if (a.IsNumber())
            {
                if (b.IsNumber())
                {
                    return TypeDescriptor.Numeric;
                }

                if (b.IsString() && op is AdditionOperator)
                {
                    return TypeDescriptor.String;
                }

                throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
            }

            if (a.IsString())
            {
                if (b.IsNumber() && op is MultiplicationOperator)
                {
                    return TypeDescriptor.String;
                }

                throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
            }

            throw new InvalidOperatorForTypeCompilerException(op.GetType(), a, b, op.Info);
        }

        public static TypeDescriptor GetDataType(this EvaluationStatement evaluationStatement, Context context,
            Scope scope)
        {
            switch (evaluationStatement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    return constantValueStatement.TypeDescriptor;
                }
                case VariableAccessStatement variableAccessStatement:
                {
                    if (scope.TryGetVariableInfo(variableAccessStatement, out var variableInfo))
                    {
                        return variableInfo.TypeDescriptor;
                    }

                    if (scope.TryGetConstantInfo(variableAccessStatement, out var constantInfo))
                    {
                        return constantInfo.TypeDescriptor;
                    }

                    if (scope.TryGetPrototypeInfo(variableAccessStatement, out var functionInfo) ||
                        scope.TryGetFunctionInfo(variableAccessStatement, out functionInfo))
                    {
                        return new TypeDescriptor(DataTypes.Delegate,
                            new TypeDescriptor.LookupInfo(functionInfo.ClassName, functionInfo.Name));
                    }

                    throw new IdentifierNotFoundCompilerException(variableAccessStatement.VariableName,
                        variableAccessStatement.Info);
                }
                case FunctionCallStatement functionCallStatement:
                {
                    var funcInfo = FunctionStatementTranspilerBase.GetFunctionInfoFromFunctionCall(context, scope,
                        functionCallStatement, out var sourceObjectInfo);

                    if (funcInfo == null)
                    {
                        throw new IdentifierNotFoundCompilerException(functionCallStatement.FunctionName,
                            functionCallStatement.Info);
                    }

                    return funcInfo.TypeDescriptor;
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

                    return TypeDescriptor.Numeric;
                }
                case LogicalEvaluationStatement _: //logicalEvaluationStatement
                {
                    return TypeDescriptor.Boolean;
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

                    return TypeDescriptor.Numeric;
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public static void AssertAssignableFrom(Context context, Scope scope, TypeDescriptor destination,
            TypeDescriptor source, StatementInfo info)
        {
            if (!IsAssignableFrom(context, scope, destination, source))
            {
                throw new TypeMismatchCompilerException(source, destination, info);
            }
        }
        
        public static bool IsAssignableFrom(Context context, Scope scope, TypeDescriptor destination,
            TypeDescriptor source)
        {
            if (destination == source)
            {
                return true;
            }

            if (destination.DataType == DataTypes.Numeric)
            {
                if (source.IsNumber())
                {
                    return true;
                }
            }
            else if (destination.IsFloat())
            {
                if (source.IsInteger())
                {
                    return true;
                }
            }

            if (destination.DataType == DataTypes.Lookup && destination.Lookup != null)
            {
                var lookup = destination.Lookup.Value;
                if (scope.TryGetPrototypeInfo(lookup.ClassName, lookup.Name, out var functionInfo) ||
                    scope.TryGetFunctionInfo(lookup.ClassName, lookup.Name, out functionInfo))
                {
                    destination = new TypeDescriptor(DataTypes.Delegate, lookup);
                }

                //TODO: support for objects, (if added later)
            }

            if (source.DataType == DataTypes.Lookup && source.Lookup != null)
            {
                var lookup = source.Lookup.Value;
                if (scope.TryGetPrototypeInfo(lookup.ClassName, lookup.Name, out var functionInfo) ||
                    scope.TryGetFunctionInfo(lookup.ClassName, lookup.Name, out functionInfo))
                {
                    source = new TypeDescriptor(DataTypes.Delegate, lookup);
                }

                //TODO: support for objects, (if added later)
            }

            if (destination.DataType == source.DataType)
            {
                if (destination.DataType == DataTypes.Delegate)
                {
                    if (destination.Lookup == null || source.Lookup == null)
                    {
                        throw new InvalidOperationException();
                    }

                    var dstLookup = destination.Lookup.Value;
                    var srcLookup = source.Lookup.Value;

                    if (!scope.TryGetPrototypeInfo(dstLookup.ClassName, dstLookup.Name, out var dstFunctionInfo) &&
                        !scope.TryGetFunctionInfo(dstLookup.ClassName, dstLookup.Name, out dstFunctionInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(dstLookup, null);
                    }

                    if (!scope.TryGetPrototypeInfo(srcLookup.ClassName, srcLookup.Name, out var srcFunctionInfo) &&
                        !scope.TryGetFunctionInfo(srcLookup.ClassName, srcLookup.Name, out srcFunctionInfo))
                    {
                        throw new IdentifierNotFoundCompilerException(dstLookup, null);
                    }

                    if (dstFunctionInfo.TypeDescriptor != srcFunctionInfo.TypeDescriptor)
                    {
                        return false;
                    }

                    if (dstFunctionInfo.Parameters != null)
                    {
                        if (srcFunctionInfo.Parameters != null)
                        {
                            for (var i = 0; i < dstFunctionInfo.Parameters.Length; i++)
                            {
                                var dstParam = dstFunctionInfo.Parameters[i];
                                var srcParam = srcFunctionInfo.Parameters[i];

                                if (dstParam.TypeDescriptor != srcParam.TypeDescriptor)
                                {
                                    return false;
                                }
                            }

                            return true;
                        }

                        return false;
                    }

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
            return dataType == DataTypes.Integer || dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Integer || dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.TypeDescriptor.DataType;
            return dataType == DataTypes.Integer || dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumericOrFloat(this DataTypes dataType)
        {
            return dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumericOrFloat(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumericOrFloat(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.TypeDescriptor.DataType;
            return dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloat(this DataTypes dataType)
        {
            return dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloat(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFloat(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteger(this DataTypes dataType)
        {
            return dataType == DataTypes.Integer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteger(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Integer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInteger(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.Integer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this DataTypes dataType)
        {
            return dataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this DataTypes dataType)
        {
            return dataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVoid(this DataTypes dataType)
        {
            return dataType == DataTypes.Void;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVoid(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Void;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVoid(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.Void;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this DataTypes dataType)
        {
            return (dataType & DataTypes.Array) == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return (dataType & DataTypes.Array) == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.TypeDescriptor.DataType;
            return (dataType & DataTypes.Array) == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this DataTypes dataType)
        {
            return dataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDelegate(this TypeDescriptor typeDescriptor)
        {
            var dataType = typeDescriptor.DataType;
            return dataType == DataTypes.Delegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDelegate(this ConstantValueStatement constantValueStatement)
        {
            return constantValueStatement.TypeDescriptor.DataType == DataTypes.Delegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDelegate(this DataTypes dataType)
        {
            return dataType == DataTypes.Delegate;
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