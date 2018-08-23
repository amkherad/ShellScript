using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IncrementStatement : EvaluationStatement
    {
        public override bool IsBlockStatement { get; }
        public override StatementInfo Info { get; }

        public VariableAccessStatement Variable { get; }
        
        public bool IsPostfix { get; }
        
        
        public IncrementStatement(VariableAccessStatement variable, bool isPostfix, StatementInfo info)
        {
            Variable = variable;
            IsPostfix = isPostfix;
            Info = info;
        }


        public override  IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                yield return Variable;
            }
        }
    }
}