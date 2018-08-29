using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class DefinitionStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public StatementInfo Info { get; }

        public DataTypes DataType { get; }
        public string Name { get; }
        
        public EvaluationStatement DefaultValue { get; }
        public bool HasDefaultValue { get; }

        public IStatement[] TraversableChildren { get; protected set; }

        
        public DefinitionStatement(DataTypes dataType, string name, EvaluationStatement defaultValue, bool hasDefaultValue, StatementInfo info)
        {
            DataType = dataType;
            Name = name;
            DefaultValue = defaultValue;
            HasDefaultValue = hasDefaultValue;
            Info = info;

            TraversableChildren = StatementHelpers.CreateChildren(defaultValue);
        }
    }
}