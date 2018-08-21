using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionParameterDefinitionStatement : DefinitionStatement
    {
        public FunctionParameterDefinitionStatement(
            DataTypes dataType, string name, ConstantValueStatement defaultValue)
            : base(dataType, name, defaultValue, defaultValue != null)
        {
        }
    }
}