using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class DefinitionStatement : IStatement
    {
        public virtual bool CanBeEmbedded => false;
        public StatementInfo Info { get; }

        public DataTypes DataType { get; }
        public string Name { get; }

        public EvaluationStatement DefaultValue { get; }
        public bool HasDefaultValue { get; }

        public IStatement[] TraversableChildren { get; protected set; }


        public DefinitionStatement(DataTypes dataType, string name, EvaluationStatement defaultValue,
            bool hasDefaultValue, StatementInfo info)
        {
            DataType = dataType;
            Name = name;
            DefaultValue = defaultValue;
            HasDefaultValue = hasDefaultValue;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(defaultValue);
        }

        public override string ToString()
        {
            if (DefaultValue != null)
            {
                return $"{DataType} {Name} = {DefaultValue}";
            }

            return $"{DataType} {Name}";
        }
    }
}