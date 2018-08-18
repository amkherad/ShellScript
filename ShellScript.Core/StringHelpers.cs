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
    }
}