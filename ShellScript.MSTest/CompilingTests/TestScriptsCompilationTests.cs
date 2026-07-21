using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class TestScriptsCompilationTests
    {
        private const string UpdateSnapshotsEnvironmentVariable = "UPDATE_SNAPSHOTS";

        [TestMethod]
        public void TestScripts()
        {
            var testScriptsRoot = FindTestScriptsRoot();
            var tempOutputRoot = Path.Combine(Path.GetDirectoryName(testScriptsRoot), "TestScripts.temp");
            var updateSnapshots = string.Equals(Environment.GetEnvironmentVariable(
                UpdateSnapshotsEnvironmentVariable), "1", StringComparison.Ordinal);
            var platforms = new[] {new UnixBashPlatform()};
            var failures = new List<string>();
            var scripts = Directory.GetFiles(testScriptsRoot, "*.shellscript", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            Assert.IsTrue(scripts.Length > 0, "No test scripts were found.");
            Directory.CreateDirectory(tempOutputRoot);

            foreach (var script in scripts)
            {
                foreach (var platform in platforms)
                {
                    var relativeSourcePath = GetRelativePath(testScriptsRoot, script);
                    var relativeOutputPath = Path.ChangeExtension(relativeSourcePath,
                        platform.Name.ToLowerInvariant());
                    var outputFile = Path.Combine(tempOutputRoot, relativeOutputPath);
                    var snapshotFile = Path.Combine(testScriptsRoot, relativeOutputPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                    try
                    {
                        using (var logWriter = new StringWriter())
                        {
                            new Compiler().CompileFromSource(script, Path.Combine(tempOutputRoot, "obj"), outputFile,
                                platform, CreateSnapshotFlags(), logWriter, logWriter, logWriter);
                        }

                        ValidateBashSyntax(outputFile);

                        var actual = Normalize(File.ReadAllText(outputFile));
                        if (updateSnapshots)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(snapshotFile));
                            File.WriteAllText(snapshotFile, actual);
                        }
                        else if (!File.Exists(snapshotFile))
                        {
                            failures.Add($"Missing snapshot: {relativeOutputPath}. Run tests with " +
                                $"{UpdateSnapshotsEnvironmentVariable}=1 to create it.");
                        }
                        else
                        {
                            var expected = Normalize(File.ReadAllText(snapshotFile));
                            if (!string.Equals(expected, actual, StringComparison.Ordinal))
                                failures.Add(CreateSnapshotFailure(relativeSourcePath, snapshotFile, outputFile,
                                    expected, actual));
                        }
                    }
                    catch (Exception exception)
                    {
                        failures.Add($"Compilation failed for {relativeSourcePath} ({platform.Name}): " +
                            exception);
                    }
                }
            }

            if (failures.Count > 0)
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, failures));
        }

        private static CompilerFlags CreateSnapshotFlags()
        {
            var flags = CompilerFlags.CreateDefault();
            flags.WriteShellScriptVersion = false;
            flags.PreferRandomHelperVariableNames = false;
            return flags;
        }

        private static string FindTestScriptsRoot()
        {
            var currentDirectory = Environment.CurrentDirectory;
            while (!string.IsNullOrWhiteSpace(currentDirectory))
            {
                if (string.Equals(Path.GetFileName(currentDirectory), "ShellScript.MSTest",
                    StringComparison.OrdinalIgnoreCase))
                    return Path.Combine(currentDirectory, "TestScripts");

                var candidate = Path.Combine(currentDirectory, "ShellScript.MSTest", "TestScripts");
                if (Directory.Exists(candidate))
                    return candidate;

                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            Assert.Fail("Test scripts directory not found.");
            return null;
        }

        private static string GetRelativePath(string root, string path)
        {
            var rootUri = new Uri(AppendDirectorySeparator(root));
            return Uri.UnescapeDataString(rootUri.MakeRelativeUri(new Uri(path)).ToString())
                .Replace('/', Path.DirectorySeparatorChar);
        }

        private static string AppendDirectorySeparator(string path)
        {
            return path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? path
                : path + Path.DirectorySeparatorChar;
        }

        private static string Normalize(string value)
        {
            return value.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        private static string CreateSnapshotFailure(string source, string snapshot, string output,
            string expected, string actual)
        {
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');
            var lineCount = Math.Max(expectedLines.Length, actualLines.Length);
            for (var index = 0; index < lineCount; index++)
            {
                var expectedLine = index < expectedLines.Length ? expectedLines[index] : "<missing>";
                var actualLine = index < actualLines.Length ? actualLines[index] : "<missing>";
                if (!string.Equals(expectedLine, actualLine, StringComparison.Ordinal))
                    return $"Snapshot mismatch for {source} at line {index + 1}." + Environment.NewLine +
                           $"Expected: {expectedLine}" + Environment.NewLine +
                           $"Actual:   {actualLine}" + Environment.NewLine +
                           $"Snapshot: {snapshot}" + Environment.NewLine +
                           $"Output:   {output}";
            }

            return $"Snapshot mismatch for {source}.";
        }

        private static void ValidateBashSyntax(string outputFile)
        {
            var startInfo = new ProcessStartInfo("bash", $"-n \"{outputFile}\"")
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(startInfo))
            {
                var standardOutput = process.StandardOutput.ReadToEnd();
                var standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new InvalidOperationException($"bash -n failed for {outputFile}: " +
                        standardOutput + standardError);
            }
        }
    }
}
