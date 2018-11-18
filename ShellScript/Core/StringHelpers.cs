using System.Text.RegularExpressions;
using ShellScript.Core.Language.Compiler.Lexing;

namespace ShellScript.Core
{
    public class StringHelpers
    {
        public static string[] SplitBySpace(string input, string splitString)
        {
            return Regex.Split(input, splitString + "(?=(?:[^']*'[^']*')*[^']*$)");
        }

        public static string[] SplitBySpace(string input, char splitChar)
        {
            return Regex.Split(input, splitChar + "(?=(?:[^']*'[^']*')*[^']*$)");
        }

        public static bool IsMultiLineString(string input)
        {
            return input.Contains('\n') || input.Contains('\r');
        }

        public static bool IsValidIdentifierName(string name)
        {
            var match = Lexer.ValidIdentifierName.Match(name);
            return match.Success;
        }
        
        public static string EnQuote(string value)
        {
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                return value;
            }

            return $"\"{value}\"";
        }
        
        public static string DeQuote(string value)
        {
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value;
        }
        
        public static string DeQuote(string value, bool deQuote)
        {
            if (!deQuote)
            {
                return value;
            }
            
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value;
        }
    }
}