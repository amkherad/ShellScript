using System;
using System.IO;
using System.Text;
using ShellScript.Core;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public static class BashTranspilerHelpers
    {
        public static void WriteComment(TextWriter writer, string comment)
        {
            writer.WriteLine("#{0}", comment);
        }
        
        public static void WriteSeparator(TextWriter writer)
        {
            writer.WriteLine("#{0}", new string('-', 79));
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

        /// <summary>
        /// Bash literal for a string value. Uses $'...' when the value contains backslash escapes (e.g. ANSI \033).
        /// </summary>
        public static string ToBashStringLiteral(string value, bool dequote = true)
        {
            var unquoted = StandardizeString(value, dequote);
            if (NeedsDollarQuotedLiteral(unquoted))
            {
                return ToBashDollarQuotedLiteral(unquoted);
            }

            return ToBashString(value, dequote, true);
        }

        private static bool NeedsDollarQuotedLiteral(string value)
        {
            if (value.IndexOf('\\', StringComparison.Ordinal) >= 0)
            {
                return true;
            }

            foreach (var c in value)
            {
                if (c < 32 || c == 127)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ToBashDollarQuotedLiteral(string value)
        {
            var sb = new StringBuilder("$'");
            foreach (var c in value)
            {
                if (c == '\'')
                {
                    sb.Append("'\\''");
                }
                else
                {
                    sb.Append(c);
                }
            }

            sb.Append('\'');
            return sb.ToString();
        }

        public static string ToBashString(string value, bool dequote, bool enquote)
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