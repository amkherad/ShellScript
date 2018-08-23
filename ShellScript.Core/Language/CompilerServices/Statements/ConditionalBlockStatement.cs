using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConditionalBlockStatement : IStatement
    {
        public bool IsBlockStatement => true;
        public StatementInfo Info { get; }

        public IStatement Condition { get; }
        public IStatement Statement { get; }
        
        
        public ConditionalBlockStatement(IStatement condition, IStatement statement, StatementInfo info)
        {
            Condition = condition;
            Statement = statement;
            Info = info;
        }


        public IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return Condition;
                yield return Statement;
            }
        }
    }
}