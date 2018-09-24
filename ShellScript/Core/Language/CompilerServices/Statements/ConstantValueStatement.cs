using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConstantValueStatement : EvaluationStatement
    {
        public override bool CanBeEmbedded => false;
        public override StatementInfo Info { get; }

        public DataTypes DataType { get; }
        public string Value { get; }


        public ConstantValueStatement(DataTypes dataType, string value,
            StatementInfo info)
        {
            DataType = dataType;
            Value = value;
            Info = info;

            TraversableChildren = new IStatement[0];
        }

        public override string ToString()
        {
            return Value;
        }
    }
}