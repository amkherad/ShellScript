using ShellScript.Core.Language.CompilerServices.Parsing;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class VariableDefinitionStatement : DefinitionStatement
    {
        public VariableDefinitionStatement(
            DataTypes dataType,
            string name,
            IStatement value,
            ParserInfo parserInfo)
            : base(dataType, name, value, true, parserInfo)
        {
        }
    }
}