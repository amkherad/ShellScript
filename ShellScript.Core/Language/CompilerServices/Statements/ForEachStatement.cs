using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ForEachStatement : IStatement
    {
        public bool IsBlockStatement => true;
        
        public IStatement Variable { get; }
        
        public VariableAccessStatement Iterator { get; }
        
        public IStatement Statement { get; }
        
        
        public ForEachStatement(IStatement variable, VariableAccessStatement iterator, IStatement statement)
        {
            Variable = variable;
            Iterator = iterator;
            Statement = statement;
        }


        public IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return Variable;
                yield return Iterator;
                yield return Statement;
            }
        }
    }
}