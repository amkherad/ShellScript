using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class DefinitionStatement : IStatement
    {
        public bool IsBlockStatement => false;

        public DataTypes DataType { get; }
        public string Name { get; }
        
        public IStatement DefaultValue { get; }
        public bool HasDefaultValue { get; }

        
        public DefinitionStatement(DataTypes dataType, string name, IStatement defaultValue, bool hasDefaultValue)
        {
            DataType = dataType;
            Name = name;
            DefaultValue = defaultValue;
            HasDefaultValue = hasDefaultValue;
        }
    }
}