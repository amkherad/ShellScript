using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Lint;
using ShellScript.Testing;

namespace ShellScript.CommandLine
{
    public class FormatCommand : ICommand
    {
        public string Name => "format";

        public Dictionary<string, string> SwitchesHelp { get; } = new Dictionary<string, string>
        {
            {
                "indent",
                $"Indent width in spaces (default {ShellScriptLintOptions.DefaultIndentSize})."
            },
        };

        public bool CanHandle(CommandContext command) => command.IsCommand("format");

        public ResultCodes Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            CommandContext context)
        {
            var options = new ShellScriptLintOptions();
            var indentSwitch = context.GetSwitch("indent");
            if (indentSwitch != null)
            {
                indentSwitch.AssertValue();
                if (!int.TryParse(indentSwitch.Value, out var indent))
                {
                    errorWriter.WriteLine($"Invalid --indent value: {indentSwitch.Value}");
                    return ResultCodes.Failure;
                }

                options.IndentSize = indent;
            }

            var pathArgs = GetPathArguments(context).ToList();
            if (pathArgs.Count == 0)
            {
                var cwd = Environment.CurrentDirectory;
                pathArgs.Add(Path.Combine(cwd, "**", "*.shellscript"));
                outputWriter.WriteLine(
                    $"No paths given; formatting under {cwd} (**/*.shellscript).");
            }
            else if (pathArgs.All(p => p.IndexOf('*', StringComparison.Ordinal) < 0) && pathArgs.Count <= 3)
            {
                warningWriter.WriteLine(
                    "Your shell may have expanded a glob before the CLI saw it. Quote the pattern, e.g. " +
                    "shellscript format 'Examples/**/*.shellscript'");
            }

            var scripts = SnapshotTester.ResolveScripts(pathArgs)
                .Where(ShellScriptSourceFilters.IsStandaloneEntry)
                .ToArray();

            if (scripts.Length == 0)
            {
                errorWriter.WriteLine("No .shellscript files matched the given path(s).");
                return ResultCodes.Failure;
            }

            foreach (var script in scripts)
            {
                var source = File.ReadAllText(script);
                var formatted = ShellScriptSourceFormatter.Format(source, options);
                if (!string.Equals(source, formatted, StringComparison.Ordinal))
                {
                    File.WriteAllText(script, formatted);
                    outputWriter.WriteLine($"Formatted: {script}");
                }
                else
                {
                    outputWriter.WriteLine($"Unchanged: {script}");
                }
            }

            return ResultCodes.Successful;
        }

        private static IEnumerable<string> GetPathArguments(CommandContext context)
        {
            var switchTokens = BuildSwitchTokenSet(context);

            for (var i = 1; i < context.Tokens.Length; i++)
            {
                var token = context.Tokens[i];
                if (switchTokens.Contains(token) || token.StartsWith("-", StringComparison.Ordinal))
                {
                    continue;
                }

                yield return token;
            }
        }

        private static HashSet<string> BuildSwitchTokenSet(CommandContext context)
        {
            var switchTokens = new HashSet<string>(StringComparer.Ordinal);
            foreach (var sw in context.Switches)
            {
                switchTokens.Add("-" + sw.Name);
                switchTokens.Add("--" + sw.Name);
                if (sw.HaveValue && sw.Value != null)
                {
                    switchTokens.Add(sw.Value);
                }
            }

            return switchTokens;
        }
    }
}
