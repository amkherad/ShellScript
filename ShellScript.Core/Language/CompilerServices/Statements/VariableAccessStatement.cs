using System.Collections.Generic;
using System.Linq;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableAccessStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        
        public string VariableName { get; }
        
        
        public VariableAccessStatement(string variableName)
        {
            VariableName = variableName;
        }


        public override IEnumerable<IStatement> TraversableChildren => Enumerable.Empty<IStatement>();
    }
}