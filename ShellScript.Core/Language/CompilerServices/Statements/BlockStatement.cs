using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class BlockStatement : IStatement
    {
        public bool IsBlockStatement => true;

        public IStatement[] Statements { get; }
        
        
        public BlockStatement(IStatement[] statements)
        {
            Statements = statements;
        }


        public IEnumerable<IStatement> TraversableChildren => Statements;
    }
}