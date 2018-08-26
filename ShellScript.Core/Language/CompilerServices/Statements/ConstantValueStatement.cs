using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConstantValueStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;
        public override StatementInfo Info { get; }

        public DataTypes DataType { get; }
        public string Value { get; }

       
        public ConstantValueStatement(DataTypes dataType, string value, StatementInfo info)
        {
            DataType = dataType;
            Value = value;
            Info = info;
            
            TraversableChildren = new IStatement[0];
        }
    }
}