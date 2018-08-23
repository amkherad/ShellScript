using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConditionalBlockStatement : IStatement
    {
        public bool IsBlockStatement => true;

        public IStatement Condition { get; }
        public IStatement Statement { get; }
        
        
        public ConditionalBlockStatement(IStatement condition, IStatement statement)
        {
            Condition = condition;
            Statement = statement;
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