using ShellScript.Core.Language.CompilerServices.Parsing;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class ConstantValueStatement : IStatement
    {
        public bool IsBlockStatement => false;
        public ParserInfo ParserInfo { get; }

        public DataTypes DataType { get; }
        public string Value { get; }

        
        public ConstantValueStatement(DataTypes dataType, string value, ParserInfo parserInfo)
        {
            DataType = dataType;
            Value = value;
            ParserInfo = parserInfo;
        }
    }
}