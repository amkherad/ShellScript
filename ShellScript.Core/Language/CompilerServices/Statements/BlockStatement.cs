using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class BlockStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }

        public IStatement[] Statements { get; }
        
        
        public BlockStatement(IStatement[] statements, StatementInfo info)
        {
            Statements = statements;
            Info = info;
        }


        public IEnumerable<IStatement> TraversableChildren => Statements;
    }
}