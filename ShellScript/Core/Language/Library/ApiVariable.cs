namespace ShellScript.Core.Language.Library
{
    public class ApiVariable : IApiVariable
    {
        public string Name { get; }
        
        public TypeDescriptor TypeDescriptor { get; }
        
        public object DefaultValue { get; }

        
        public ApiVariable(string name, TypeDescriptor typeDescriptor)
        {
            Name = name;
            TypeDescriptor = typeDescriptor;
        }
        
        public ApiVariable(string name, TypeDescriptor typeDescriptor, object defaultValue)
        {
            Name = name;
            TypeDescriptor = typeDescriptor;
            DefaultValue = defaultValue;
        }
    }
}