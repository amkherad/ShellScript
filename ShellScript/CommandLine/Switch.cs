using System;
using System.Text.RegularExpressions;

namespace ShellScript.CommandLine
{
    public class Switch
    {
        public string Name { get; }

        public string Value { get; }

        public bool HaveValue { get; }

        public Switch(string name, string value, bool haveValue)
        {
            Name = name;
            Value = value;
            HaveValue = haveValue;
        }

        public static Switch Parse(string s)
        {
            var regex = new Regex(@"^-[-]?[\S]+");
            var match = regex.Match(s);
            if (match.Success)
            {
                var regexValue = new Regex(@"=.*");
                var matchValue = regexValue.Match(s);
                if (matchValue.Success)
                {
                    var value = matchValue.Value.Substring(1);
                    if (value == "-null")
                    {
                        value = null;
                    }

                    return new Switch(
                        match.Value.Substring(0, match.Value.IndexOf('=')).TrimStart('-'),
                        value,
                        true
                    );
                }

                return new Switch(match.Value.TrimStart('-'), null, false);
            }

            throw new InvalidOperationException($"Invalid switch '{s}' provided.");
        }

        public void AssertValue()
        {
            if (!HaveValue)
            {
                throw new Exception($"Switch '{Name}' must have a value.");
            }
        }

        public void AssertValue(string failMessage)
        {
            if (!HaveValue)
            {
                throw new Exception($"Switch '{Name}' must have a value, {failMessage}");
            }
        }
    }
}