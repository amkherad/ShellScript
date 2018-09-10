namespace ShellScript.Core.Language.Library
{
    public class ApiVariable : IApiVariable
    {
        public string Name { get; }
        
        public DataTypes DataType { get; }
        
        public object DefaultValue { get; }

        
        public ApiVariable(string name, DataTypes dataType)
        {
            Name = name;
            DataType = dataType;
        }
        
        public ApiVariable(string name, DataTypes dataType, object defaultValue)
        {
            Name = name;
            DataType = dataType;
            DefaultValue = defaultValue;
        }
    }
}