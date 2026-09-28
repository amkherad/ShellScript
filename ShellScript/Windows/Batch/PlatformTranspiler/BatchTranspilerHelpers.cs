using System.IO;
using ShellScript.Core;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Windows.Batch.PlatformTranspiler
{
    public static class BatchTranspilerHelpers
    {
        public static void WriteComment(TextWriter writer, string comment)
        {
            writer.WriteLine("REM {0}", comment);
        }
        
        public static void WriteSeparator(TextWriter writer)
        {
            writer.WriteLine("REM {0}", new string('-', 79));
        }

        public static string StandardizeString(string value, bool deQuote)
        {
            if (deQuote)
            {
                value = StringHelpers.DeQuote(value);
            }

            return value.Replace(@"\r\n", @"\n");
        }

        public static string GetString(string value)
        {
            value = StringHelpers.DeQuote(value);

            return value;//.Replace(@"\r\n", @"\n");
        }

        public static string ToBatchString(string value, bool dequote, bool enquote)
        {
            value = StandardizeString(value, dequote);

            if (value.Contains('\\'))
            {
                value = value.Replace("\\", "\\\\");
            }

            if (value.Contains('"'))
            {
                value = value.Replace("\"", "\\\"");
            }
            
            if (value.Contains('`'))
            {
                value = value.Replace("`", "\\`");
            }

            if (value.Contains('\r'))
            {
                value = value.Replace("\r", "\\r");
            }

            if (value.Contains('\n'))
            {
                value = value.Replace("\n", "\\n");
            }

            if (value.Contains('$'))
            {
                value = value.Replace("$", "\\$");
            }

            return enquote
                ? $"\"{value}\""
                : value;
        }

        public static InvalidStatementStructureCompilerException InvalidStatementStructure(Scope scope,
            EvaluationStatement statement)
        {
            return new InvalidStatementStructureCompilerException(statement, statement?.Info);
        }
    }
}