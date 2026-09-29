using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Core.Language.Compiler;

namespace ShellScript.Testing
{
    public sealed class SnapshotTestResult
    {
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Skipped { get; set; }
        public List<SnapshotTestFailure> Failures { get; } = new List<SnapshotTestFailure>();
    }

    public static class SnapshotTester
    {
        public const string UpdateSnapshotsEnvironmentVariable = "UPDATE_SNAPSHOTS";

        public static SnapshotTestResult RunOutputSnapshots(
            IEnumerable<string> pathPatterns,
            SnapshotTestOptions options,
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter)
        {
            var result = new SnapshotTestResult();
            var scripts = ResolveScripts(pathPatterns).ToArray();

            if (scripts.Length == 0)
            {
                result.Failures.Add(new SnapshotTestFailure("(none)",
                    "No .shellscript files matched the given path(s)."));
                result.Failed = 1;
                return result;
            }

            var updateSnapshots = ShouldUpdateSnapshots(options);
            var patterns = pathPatterns.ToArray();
            var explicitSingleFile = patterns.Length == 1 && !patterns[0].Contains("*");

            foreach (var script in scripts)
            {
                var snapshotPath = SnapshotPaths.GetOutputSnapshotPath(script, options.PlatformName);
                if (!File.Exists(snapshotPath) && !updateSnapshots)
                {
                    if (explicitSingleFile && string.Equals(Path.GetFullPath(patterns[0]), script, StringComparison.Ordinal))
                    {
                        result.Failed++;
                        result.Failures.Add(new SnapshotTestFailure(script,
                            $"Missing snapshot file '{snapshotPath}'. Run with --update-snapshots or " +
                            $"{UpdateSnapshotsEnvironmentVariable}=1 to create it."));
                    }
                    else
                    {
                        result.Skipped++;
                    }

                    continue;
                }

                try
                {
                    var runResult = ShellScriptRunner.Run(script, options.PlatformName, null, errorWriter,
                        warningWriter, logWriter);
                    var actual = Normalize(runResult.StandardOutput);

                    if (updateSnapshots)
                    {
                        File.WriteAllText(snapshotPath, actual);
                        result.Passed++;
                        outputWriter?.WriteLine($"Updated snapshot: {snapshotPath}");
                        continue;
                    }

                    var expected = Normalize(File.ReadAllText(snapshotPath));
                    if (string.Equals(expected, actual, StringComparison.Ordinal))
                    {
                        result.Passed++;
                    }
                    else
                    {
                        result.Failed++;
                        result.Failures.Add(new SnapshotTestFailure(script,
                            CreateOutputMismatchMessage(script, snapshotPath, expected, actual)));
                    }
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    result.Failures.Add(new SnapshotTestFailure(script, ex.ToString()));
                }
            }

            return result;
        }

        public static bool ShouldUpdateSnapshots(SnapshotTestOptions options) =>
            options.UpdateSnapshots || string.Equals(
                Environment.GetEnvironmentVariable(UpdateSnapshotsEnvironmentVariable), "1",
                StringComparison.Ordinal);

        public static IEnumerable<string> ResolveScripts(IEnumerable<string> pathPatterns)
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            foreach (var pattern in pathPatterns)
            {
                foreach (var path in PathGlob.Expand(pattern))
                {
                    if (path.EndsWith(".shellscript", StringComparison.OrdinalIgnoreCase))
                    {
                        set.Add(path);
                    }
                }
            }

            return set.OrderBy(path => path, StringComparer.Ordinal);
        }

        public static string Normalize(string value) =>
            value.Replace("\r\n", "\n").Replace('\r', '\n');

        private static string CreateOutputMismatchMessage(string script, string snapshotPath, string expected,
            string actual)
        {
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');
            var lineCount = Math.Max(expectedLines.Length, actualLines.Length);
            for (var index = 0; index < lineCount; index++)
            {
                var expectedLine = index < expectedLines.Length ? expectedLines[index] : "<missing>";
                var actualLine = index < actualLines.Length ? actualLines[index] : "<missing>";
                if (!string.Equals(expectedLine, actualLine, StringComparison.Ordinal))
                {
                    return "Output snapshot mismatch at line " + (index + 1) + "." + Environment.NewLine +
                           "Expected: " + expectedLine + Environment.NewLine +
                           "Actual:   " + actualLine + Environment.NewLine +
                           "Snapshot: " + snapshotPath;
                }
            }

            return "Output snapshot mismatch.";
        }
    }
}
