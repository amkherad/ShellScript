using System.Text.RegularExpressions;

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
    }
}