using ShellScript.Core.Language.Compiler.Parsing;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class DefinitionStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public ParserInfo ParserInfo { get; }

        public DataTypes DataType { get; }
        public string Name { get; }
        
        public IStatement DefaultValue { get; }
        public bool HasDefaultValue { get; }

        
        public DefinitionStatement(DataTypes dataType, string name, IStatement defaultValue, bool hasDefaultValue, ParserInfo parserInfo)
        {
            DataType = dataType;
            Name = name;
            DefaultValue = defaultValue;
            HasDefaultValue = hasDefaultValue;
            ParserInfo = parserInfo;
        }
    }
}