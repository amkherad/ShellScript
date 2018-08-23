using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class EvaluationStatement : IStatement
    {
        public abstract bool IsBlockStatement { get; }
        public abstract StatementInfo Info { get; }

        public abstract IEnumerable<IStatement> TraversableChildren { get; }
    }
}