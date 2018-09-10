using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public static class StatementHelpers
    {
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

        private static bool _traverseTreeContains<TStatement>(IStatement statement)
            where TStatement : IStatement
        {
            foreach (var child in statement.TraversableChildren)
            {
                if (child is TStatement)
                {
                    return true;
                }

                if (_traverseTreeContains<TStatement>(child))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool _traverseTreeContains(IStatement statement, Type[] types)
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

                if (_traverseTreeContains(child, types))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TreeContains<TStatement>(this IStatement statement)
            where TStatement : IStatement
        {
            return _traverseTreeContains<TStatement>(statement);
        }

        public static bool TreeContains(this IStatement statement, params Type[] types)
        {
            return _traverseTreeContains(statement, types);
        }


        private static bool _traverseTreeAny(IStatement statement, Predicate<IStatement> predicate)
        {
            foreach (var child in statement.TraversableChildren)
            {
                if (predicate(child))
                {
                    return true;
                }

                if (_traverseTreeAny(child, predicate))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TreeAny(this IStatement statement, Predicate<IStatement> predicate)
        {
            return _traverseTreeAny(statement, predicate);
        }

        private static bool _traverseTreeAll(IStatement statement, Predicate<IStatement> predicate)
        {
            var atLeastOneExecuted = false;
            foreach (var child in statement.TraversableChildren)
            {
                if (predicate(child))
                {
                    return false;
                }

                if (_traverseTreeAny(child, predicate))
                {
                    return false;
                }

                atLeastOneExecuted = true;
            }

            return atLeastOneExecuted;
        }

        public static bool TreeAll(this IStatement statement, Predicate<IStatement> predicate)
        {
            return _traverseTreeAll(statement, predicate);
        }

//        public static IStatement[] CreateChildren(IStatement firstChild, IStatement[] children)
//        {
//            var result = new List<IStatement>(children.Length + 1);
//            
//            if (firstChild != null)
//                result.Add(firstChild);
//            
//            foreach (var child in children)
//            {
//                if (child != null)
//                    result.Add(child);
//            }
//
//            return result.ToArray();
//        }

        public static DataTypes GetDataType(this EvaluationStatement evaluationStatement)
        {
            return DataTypes.Numeric;
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumeric(this DataTypes dataType)
        {
            return dataType == DataTypes.Decimal || dataType == DataTypes.Numeric || dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumeric(this ConstantValueStatement constantValueStatement)
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
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Float;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDecimal(this DataTypes dataType)
        {
            return dataType == DataTypes.Decimal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDecimal(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Decimal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this DataTypes dataType)
        {
            return dataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBoolean(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this DataTypes dataType)
        {
            return dataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsString(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.String;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this DataTypes dataType)
        {
            return dataType == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsArray(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this DataTypes dataType)
        {
            return dataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsObject(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
            return dataType == DataTypes.Class;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFunction(this DataTypes dataType)
        {
            return dataType == DataTypes.Delegate;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFunction(this ConstantValueStatement constantValueStatement)
        {
            var dataType = constantValueStatement.DataType;
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