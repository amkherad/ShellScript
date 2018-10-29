using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public class FunctionParameterDefinitionStatement : DefinitionStatement
    {
        public FunctionParameterDefinitionStatement(
            TypeDescriptor typeDescriptor, string name, ConstantValueStatement defaultValue, StatementInfo info)
            : base(typeDescriptor, name, defaultValue, defaultValue != null, info)
        {
        }
    }
}