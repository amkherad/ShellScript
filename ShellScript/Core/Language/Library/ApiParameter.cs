namespace ShellScript.Core.Language.Library
{
    public class ApiParameter : IApiParameter
    {
        public string Name { get; }
        
        public DataTypes DataType { get; }
        
        public bool IsByRef { get; }
        
        public string Documentation { get; }
        
        
        public ApiParameter(string name, DataTypes dataType, string doc)
        {
            Name = name;
            DataType = dataType;
            Documentation = doc;
        }
        
        public ApiParameter(string name, DataTypes dataType, bool isByRef, string doc)
        {
            Name = name;
            DataType = dataType;
            IsByRef = isByRef;
            Documentation = doc;
        }
    }
}