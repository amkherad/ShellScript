using System.Collections.Generic;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class IncrementStatement : EvaluationStatement
    {
        public override bool IsBlockStatement { get; }
        
        public VariableAccessStatement Variable { get; }
        
        public bool IsPostfix { get; }
        
        
        public IncrementStatement(VariableAccessStatement variable, bool isPostfix)
        {
            Variable = variable;
            IsPostfix = isPostfix;
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