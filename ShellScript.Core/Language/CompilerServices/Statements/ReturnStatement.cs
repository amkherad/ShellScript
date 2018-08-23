using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ReturnStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public StatementInfo Info { get; }

        public EvaluationStatement Statement { get; }
        
        
        public ReturnStatement(EvaluationStatement statement, StatementInfo info)
        {
            Statement = statement;
            Info = info;
        }

        
        public IEnumerable<IStatement> TraversableChildren
        {
            get { yield return Statement; }
        }
    }
}