using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Testing;

namespace ShellScript.CommandLine
{
    public class TestCommand : ICommand
    {
        public string Name => "Test";

        public Dictionary<string, string> SwitchesHelp { get; } = new Dictionary<string, string>
        {
            {"snapshot", "Run .shellscript files and compare stdout to sibling .output.txt snapshots."},
            {"update-snapshots", "Write captured stdout to .output.txt (also honored when UPDATE_SNAPSHOTS=1)."},
            {"platform", "Target platform name (default: Unix-Bash). Example: --platform=Unix-Bash"}
        };

        public int ProcessExitCode { get; private set; }

        public bool CanHandle(CommandContext command) => command.IsCommand("test");

        public ResultCodes Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            ProcessExitCode = 1;

            if (!context.AnySwitch("snapshot"))
            {
                errorWriter.WriteLine("Specify --snapshot and optional .shellscript paths or globs.");
                errorWriter.WriteLine("With no paths, all **/*.shellscript files under the current directory are tested.");
                errorWriter.WriteLine("Quote globs so the CLI expands them: shellscript test --snapshot 'Examples/**/*.shellscript'");
                return ResultCodes.Failure;
            }

            var paths = GetPathArguments(context).ToArray();
            if (paths.Length == 0)
            {
                var cwd = Environment.CurrentDirectory;
                paths = new[]
                {
                    Path.Combine(cwd, "**", "*.shellscript"),
                };
                outputWriter.WriteLine(
                    $"No paths given; discovering snapshots under {cwd} (**/*.shellscript).");
            }
            else if (paths.All(p => p.IndexOf('*', StringComparison.Ordinal) < 0) && paths.Length <= 3)
            {
                warningWriter.WriteLine(
                    "Your shell may have expanded a glob before the CLI saw it (often only a few files). " +
                    "Run `shellscript test --snapshot` with no paths to scan the whole tree, " +
                    "or quote the pattern: shellscript test --snapshot './**/*.shellscript'");
            }

            var options = new SnapshotTestOptions
            {
                PlatformName = context.GetSwitch("platform")?.Value ?? ShellScriptRunner.DefaultPlatformName,
                UpdateSnapshots = context.AnySwitch("update-snapshots") ||
                                  string.Equals(
                                      Environment.GetEnvironmentVariable(SnapshotTester.UpdateSnapshotsEnvironmentVariable),
                                      "1", StringComparison.Ordinal),
            };

            var testResult = SnapshotTester.RunOutputSnapshots(paths, options, outputWriter, errorWriter,
                warningWriter, logWriter);

            outputWriter.WriteLine(
                $"Snapshot tests: {testResult.Passed} passed, {testResult.Failed} failed, {testResult.Skipped} skipped.");

            foreach (var failure in testResult.Failures)
            {
                errorWriter.WriteLine(failure);
            }

            ProcessExitCode = testResult.Failed > 0 ? 1 : 0;
            return testResult.Failed > 0 ? ResultCodes.Failure : ResultCodes.Successful;
        }

        private static IEnumerable<string> GetPathArguments(CommandContext context)
        {
            var switchTokens = BuildSwitchTokenSet(context);

            for (var i = 1; i < context.Tokens.Length; i++)
            {
                var token = context.Tokens[i];
                if (switchTokens.Contains(token) || token.StartsWith("-", System.StringComparison.Ordinal))
                {
                    continue;
                }

                yield return token;
            }
        }

        private static HashSet<string> BuildSwitchTokenSet(CommandContext context)
        {
            var switchTokens = new HashSet<string>(System.StringComparer.Ordinal);
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
