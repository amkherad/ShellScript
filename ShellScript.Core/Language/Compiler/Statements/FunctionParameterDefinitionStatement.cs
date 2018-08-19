using ShellScript.Core.Language.Compiler.Parsing;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class FunctionParameterDefinitionStatement : DefinitionStatement
    {
        public FunctionParameterDefinitionStatement(
            DataTypes dataType, string name, ConstantValueStatement defaultValue, ParserInfo info)
            : base(dataType, name, defaultValue, defaultValue != null, info)
        {
        }
    }
}