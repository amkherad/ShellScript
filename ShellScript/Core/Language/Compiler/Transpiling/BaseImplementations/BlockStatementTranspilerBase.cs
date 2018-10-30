using System;
using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations
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
            if (statement.CanBeEmbedded && !(statement is BlockStatement)) return true;

            foreach (var child in statement.TraversableChildren)
            {
                if (_isBlockStatement(child))
                    return true;
            }
            
            return false;
        }

        public static bool IsEmptyBody(IStatement statement)
        {
            return !_isBlockStatement(statement);
        }
    }
}