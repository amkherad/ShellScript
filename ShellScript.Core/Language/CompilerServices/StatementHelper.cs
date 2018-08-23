using System;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices
{
    public static class StatementHelper
    {
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
        
        public static bool TraverseTreeContains<TStatement>(this IStatement statement)
            where TStatement : IStatement
        {
            return _traverseTreeContains<TStatement>(statement);
        }
        
        public static bool TraverseTreeContains(this IStatement statement, params Type[] types)
        {
            return _traverseTreeContains(statement, types);
        }
    }
}