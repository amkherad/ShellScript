using System.Collections.Generic;
using System.Linq;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConstantValueStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;

        public DataTypes DataType { get; }
        public string Value { get; }

        
        public ConstantValueStatement(DataTypes dataType, string value)
        {
            DataType = dataType;
            Value = value;
        }


        public override IEnumerable<IStatement> TraversableChildren => Enumerable.Empty<IStatement>();
    }
}