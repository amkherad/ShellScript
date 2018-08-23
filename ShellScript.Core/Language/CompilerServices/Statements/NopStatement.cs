using System.Collections.Generic;
using System.Linq;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class NopStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        public override StatementInfo Info { get; }

        public NopStatement(StatementInfo info)
        {
            Info = info;
        }


        public override IEnumerable<IStatement> TraversableChildren => Enumerable.Empty<IStatement>();
    }
}