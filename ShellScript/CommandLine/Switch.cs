namespace ShellScript.CommandLine
{
    public class Switch
    {
        public string Name { get; }
        
        public string Activator { get; }
        
        public string Value { get; }
        
        
        public Switch(string name, string activator, string value)
        {
            Name = name;
            Activator = activator;
            Value = value;
        }
    }
}