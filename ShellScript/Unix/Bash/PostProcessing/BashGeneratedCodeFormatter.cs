using System;
using System.Collections.Generic;
using System.Text;

namespace ShellScript.Unix.Bash.PostProcessing
{
    /// <summary>
    /// Indents ShellScript-generated Bash using block keywords (if/then/fi, while/do/done, function braces, case/esac).
    /// </summary>
    internal static class BashGeneratedCodeFormatter
    {
        public static string Format(string source, int indentColumns)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            var width = indentColumns < 0 ? 0 : indentColumns;
            var lines = SplitLinesPreserveLastEmpty(source);
            var depth = 0;
            var pendingThen = false;
            var pendingDo = false;
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

                var trimmed = raw.TrimEnd();
                if (trimmed.Length == 0)
                {
                    output.AppendLine();
                    continue;
                }

                var content = trimmed.TrimStart();
                var first = GetFirstToken(content);

                if (IsClosingKeyword(first, content))
                {
                    depth = Math.Max(0, depth - 1);
                    pendingThen = false;
                    pendingDo = false;
                }
                else if (first == "else" || first == "elif")
                {
                    depth = Math.Max(0, depth - 1);
                    pendingThen = false;
                    pendingDo = false;
                }

                if (width > 0 && depth > 0)
                {
                    output.Append(new string(' ', depth * width));
                }

                output.Append(trimmed);

                if (i < lines.Count - 1 || endsWithNewline)
                {
                    output.AppendLine();
                }

                ApplyDepthAfterLine(content, first, ref depth, ref pendingThen, ref pendingDo);
            }

            return output.ToString();
        }

        private static void ApplyDepthAfterLine(
            string content,
            string first,
            ref int depth,
            ref bool pendingThen,
            ref bool pendingDo)
        {
            if (IsClosingKeyword(first, content))
            {
                return;
            }

            if (first == "else")
            {
                depth++;
                pendingThen = false;
                pendingDo = false;
                return;
            }

            if (first == "elif")
            {
                if (HasInlineThen(content) && !HasInlineFi(content))
                {
                    depth++;
                }
                else if (!HasInlineThen(content))
                {
                    pendingThen = true;
                }

                pendingDo = false;
                return;
            }

            if (first == "then" && content == "then")
            {
                if (pendingThen)
                {
                    depth++;
                }

                pendingThen = false;
                pendingDo = false;
                return;
            }

            if (first == "do" && content == "do")
            {
                if (pendingDo)
                {
                    depth++;
                }

                pendingThen = false;
                pendingDo = false;
                return;
            }

            if (first == "function" && content.EndsWith("{", StringComparison.Ordinal))
            {
                depth++;
                pendingThen = false;
                pendingDo = false;
                return;
            }

            if (first == "case" && content.EndsWith(" in", StringComparison.Ordinal))
            {
                depth++;
                pendingThen = false;
                pendingDo = false;
                return;
            }

            if (first == "if" || first == "while" || first == "until" || first == "for")
            {
                if (HasInlineThen(content))
                {
                    if (!HasInlineFi(content))
                    {
                        depth++;
                    }

                    pendingThen = false;
                }
                else if (first == "if")
                {
                    pendingThen = true;
                }

                if (HasInlineDo(content))
                {
                    if (!HasInlineDone(content))
                    {
                        depth++;
                    }

                    pendingDo = false;
                }
                else if (first == "while" || first == "until" || first == "for")
                {
                    pendingDo = true;
                }

                return;
            }
        }

        private static bool IsClosingKeyword(string first, string content)
        {
            if (first == "fi" || first == "done" || first == "esac")
            {
                return true;
            }

            return first == "}" && content == "}";
        }

        private static bool HasInlineThen(string content) => ContainsOutsideSingleQuotes(content, " then");

        private static bool HasInlineFi(string content) =>
            ContainsOutsideSingleQuotes(content, " fi") || ContainsOutsideSingleQuotes(content, ";fi");

        private static bool HasInlineDo(string content) =>
            ContainsOutsideSingleQuotes(content, " do") || ContainsOutsideSingleQuotes(content, ";do");

        private static bool HasInlineDone(string content) =>
            ContainsOutsideSingleQuotes(content, " done") || ContainsOutsideSingleQuotes(content, ";done");

        private static bool ContainsOutsideSingleQuotes(string content, string token)
        {
            var idx = 0;
            while (idx <= content.Length - token.Length)
            {
                idx = content.IndexOf(token, idx, StringComparison.Ordinal);
                if (idx < 0)
                {
                    return false;
                }

                if (!IsInsideSingleQuotes(content, idx))
                {
                    return true;
                }

                idx += token.Length;
            }

            return false;
        }

        private static bool IsInsideSingleQuotes(string text, int position)
        {
            var inQuote = false;
            for (var i = 0; i < position && i < text.Length; i++)
            {
                if (text[i] == '\'' && (i == 0 || text[i - 1] != '\\'))
                {
                    inQuote = !inQuote;
                }
            }

            return inQuote;
        }

        private static string GetFirstToken(string content)
        {
            var i = 0;
            while (i < content.Length && char.IsWhiteSpace(content[i]))
            {
                i++;
            }

            var start = i;
            while (i < content.Length && !char.IsWhiteSpace(content[i]))
            {
                i++;
            }

            return i <= start ? string.Empty : content.Substring(start, i - start);
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

                var end = i;
                if (end > start && source[end - 1] == '\r')
                {
                    end--;
                }

                lines.Add(source.Substring(start, end - start));
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
