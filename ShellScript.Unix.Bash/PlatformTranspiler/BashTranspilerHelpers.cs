using System.IO;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;

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

        public static InvalidStatementStructureCompilerException InvalidStatementStructure(Scope scope, EvaluationStatement statement)
        {
            return new InvalidStatementStructureCompilerException(statement, statement?.Info);
        }
    }
}