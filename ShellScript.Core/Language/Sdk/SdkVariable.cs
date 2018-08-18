namespace ShellScript.Core.Language.Sdk
{
    public class SdkVariable : ISdkVariable
    {
        public string Name { get; }
        
        public DataTypes DataType { get; }
        
        public object DefaultValue { get; }

        
        public SdkVariable(string name, DataTypes dataType)
        {
            Name = name;
            DataType = dataType;
        }
        
        public SdkVariable(string name, DataTypes dataType, object defaultValue)
        {
            Name = name;
            DataType = dataType;
            DefaultValue = defaultValue;
        }
    }
}