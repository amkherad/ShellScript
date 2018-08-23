using System.Collections.Generic;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class DefinitionStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public StatementInfo Info { get; }

        public DataTypes DataType { get; }
        public string Name { get; }
        
        public IStatement DefaultValue { get; }
        public bool HasDefaultValue { get; }

        
        public DefinitionStatement(DataTypes dataType, string name, IStatement defaultValue, bool hasDefaultValue, StatementInfo info)
        {
            DataType = dataType;
            Name = name;
            DefaultValue = defaultValue;
            HasDefaultValue = hasDefaultValue;
            Info = info;
        }


        public IEnumerable<IStatement> TraversableChildren
        {
            get
            {
                if (DefaultValue != null) yield return DefaultValue;
            }
        }
    }
}