using System;
using System.Collections.Generic;
using System.Text;

namespace ShellScript.Lint
{
    /// <summary>
    /// Reformats ShellScript source using brace depth and configured indent width.
    /// </summary>
    public static class ShellScriptSourceFormatter
    {
        public static string Format(string source, ShellScriptLintOptions options)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            var width = options?.IndentSize ?? ShellScriptLintOptions.DefaultIndentSize;
            if (width < 0)
            {
                width = 0;
            }

            var lines = SplitLinesPreserveLastEmpty(source);
            var depth = 0;
            var output = new StringBuilder(source.Length + lines.Count * (width + 2));
            var endsWithNewline = source.EndsWith("\n", StringComparison.Ordinal)
                                  || source.EndsWith("\r\n", StringComparison.Ordinal);

            for (var i = 0; i < lines.Count; i++)
            {
                var raw = lines[i];
                if (raw.Length == 0)
                {
                    output.AppendLine();
                    continue;
                }

                var trimmed = raw.Trim();
                if (trimmed.Length == 0)
                {
                    output.AppendLine();
                    continue;
                }

                var depthForLine = depth;
                depthForLine = Math.Max(0, depthForLine - CountLeadingCloseBraces(trimmed));

                if (width > 0 && depthForLine > 0)
                {
                    output.Append(new string(' ', depthForLine * width));
                }

                output.Append(trimmed);

                if (i < lines.Count - 1 || endsWithNewline)
                {
                    output.AppendLine();
                }

                depth += CountBraceDelta(trimmed);
                if (depth < 0)
                {
                    depth = 0;
                }
            }

            return output.ToString();
        }

        private static int CountLeadingCloseBraces(string trimmed)
        {
            var count = 0;
            var i = 0;
            while (i < trimmed.Length)
            {
                if (char.IsWhiteSpace(trimmed[i]))
                {
                    i++;
                    continue;
                }

                if (trimmed[i] != '}')
                {
                    break;
                }

                count++;
                i++;
            }

            return count;
        }

        private static int CountBraceDelta(string line)
        {
            var delta = 0;
            char? inString = null;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (inString != null)
                {
                    if (c == '\\' && i + 1 < line.Length)
                    {
                        i++;
                        continue;
                    }

                    if (c == inString)
                    {
                        inString = null;
                    }

                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    inString = c;
                    continue;
                }

                if (c == '/' && i + 1 < line.Length && line[i + 1] == '/')
                {
                    break;
                }

                if (c == '{')
                {
                    delta++;
                }
                else if (c == '}')
                {
                    delta--;
                }
            }

            return delta;
        }

        private static List<string> SplitLinesPreserveLastEmpty(string source)
        {
            var lines = new List<string>();
            var start = 0;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] != '\n')
                {
                    continue;
                }

                var lineEnd = i;
                if (lineEnd > start && source[lineEnd - 1] == '\r')
                {
                    lineEnd--;
                }

                lines.Add(source.Substring(start, lineEnd - start));
                start = i + 1;
            }

            if (start < source.Length)
            {
                lines.Add(source.Substring(start));
            }

            return lines;
        }
    }
}
