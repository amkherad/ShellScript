using System.IO;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public static class BashTranspilerHelpers
    {
        public static void WriteComment(TextWriter writer, string comment)
        {
            writer.WriteLine("#{0}", comment);
        }

        public static string StandardizeString(string value, bool deQuote)
        {
            if (deQuote && value[0] == '"' && value[value.Length - 1] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            return value.Replace(@"\r\n", @"\n");
        }
    }
}