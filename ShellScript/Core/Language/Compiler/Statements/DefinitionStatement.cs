using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public abstract class DefinitionStatement : IStatement
    {
        public virtual bool CanBeEmbedded => false;
        public StatementInfo Info { get; }

        public TypeDescriptor TypeDescriptor { get; }
        public string Name { get; }

        public EvaluationStatement DefaultValue { get; }
        public bool HasDefaultValue { get; }

        public IStatement[] TraversableChildren { get; protected set; }


        public DefinitionStatement(TypeDescriptor typeDescriptor, string name, EvaluationStatement defaultValue,
            bool hasDefaultValue, StatementInfo info)
        {
            TypeDescriptor = typeDescriptor;
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
                return $"{TypeDescriptor} {Name} = {DefaultValue}";
            }

            return $"{TypeDescriptor} {Name}";
        }
    }
}