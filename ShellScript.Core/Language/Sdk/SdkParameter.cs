namespace ShellScript.Core.Language.Sdk
{
    public class SdkParameter : ISdkParameter
    {
        public string Name { get; }
        
        public DataTypes DataType { get; }
        
        public bool IsByRef { get; }
        
        
        public SdkParameter(string name, DataTypes dataType)
        {
            Name = name;
            DataType = dataType;
        }
        
        public SdkParameter(string name, DataTypes dataType, bool isByRef)
        {
            Name = name;
            DataType = dataType;
            IsByRef = isByRef;
        }
    }
}