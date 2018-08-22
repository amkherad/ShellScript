using System;
using System.Text.RegularExpressions;

namespace ShellScript.CommandLine
{
    public class Switch
    {
        public string Name { get; }
        
        public string Value { get; }
        
        
        public Switch(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static Switch Parse(string s)
        {
            var regex = new Regex(@"^-[-]\w+");
            var match = regex.Match(s);
            if (match.Success)
            {
                var regexValue = new Regex(@"=.*");
                var matchValue = regexValue.Match(s);
                if (matchValue.Success)
                {
                    return new Switch(match.Value, matchValue.Value);
                }
                return new Switch(match.Value, null);
            }
            
            throw new InvalidOperationException($"Invalid switch '{s}' provided.");
        }
    }
}