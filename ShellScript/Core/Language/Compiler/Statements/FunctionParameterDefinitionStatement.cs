using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Statements
{
    public class FunctionParameterDefinitionStatement : DefinitionStatement
    {
        public bool DynamicType { get; }
        
        public FunctionParameterDefinitionStatement(
            TypeDescriptor typeDescriptor, string name, ConstantValueStatement defaultValue, StatementInfo info)
            : base(typeDescriptor, name, defaultValue, defaultValue != null, info)
        {
        }
        
        public FunctionParameterDefinitionStatement(
            TypeDescriptor typeDescriptor, string name, ConstantValueStatement defaultValue, StatementInfo info, bool dynamicType)
            : base(typeDescriptor, name, defaultValue, defaultValue != null, info)
        {
            DynamicType = dynamicType;
        }
    }
}