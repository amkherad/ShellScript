using System;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations
{
    public abstract class BlockStatementTranspilerBase : StatementTranspilerBase
    {
        public override Type StatementType => typeof(BlockStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        private static bool _isBlockStatement(IStatement statement)
        {
            if (statement.IsBlockStatement && !(statement is BlockStatement)) return true;

            foreach (var child in statement.TraversableChildren)
            {
                if (_isBlockStatement(child))
                    return true;
            }
            
            return false;
        }

        public static bool IsEmptyBody(BlockStatement statement)
        {
            return !_isBlockStatement(statement);
        }
    }
}