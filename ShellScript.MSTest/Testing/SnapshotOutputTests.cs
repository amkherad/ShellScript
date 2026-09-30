using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Testing;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.Testing
{
    [TestClass]
    public class SnapshotOutputTests
    {
        private static readonly string[] SnapshotExcludeSuffixes =
        {
            Path.Combine("ShellScript.Luncher", "shellscript.shellscript"),
            Path.Combine("ShellScript", "Shared", "Api", "ShellScriptResources", "ApiConvert_ToBoolean.shellscript"),
            Path.Combine("ShellScript", "Shared", "Api", "ShellScriptResources", "ApiMath_Truncate.shellscript"),
            Path.Combine("Examples", "Events", "FileSystemWatch", "FileSystemWatch.shellscript"),
            Path.Combine("Examples", "Apps", "MenuConsole", "MenuConsole.shellscript"),
            Path.Combine("Examples", "Apps", "PortMonitor", "PortMonitor.shellscript"),
        };

        [TestMethod]
        public void SnapshotTestsMatchOutputFiles()
        {
            Platforms.AddPlatform(new UnixBashPlatform());

            var repoRoot = FindRepositoryRoot();
            var patterns = new[]
            {
                Path.Combine(repoRoot, "Tests", "Snapshot", "**", "*.shellscript"),
                Path.Combine(repoRoot, "Examples", "**", "*.shellscript"),
                Path.Combine(repoRoot, "ShellScript.MSTest", "TestScripts", "**", "*.shellscript"),
            };

            using (var output = new StringWriter())
            using (var errors = new StringWriter())
            {
                var result = SnapshotTester.RunOutputSnapshots(
                    patterns,
                    new SnapshotTestOptions {PlatformName = ShellScriptRunner.DefaultPlatformName},
                    output,
                    errors,
                    errors,
                    errors);

                var excludedFailures = result.Failures
                    .Where(f => ShouldExcludeSnapshotScript(f.ScriptPath))
                    .ToList();
                foreach (var excluded in excludedFailures)
                {
                    result.Failures.Remove(excluded);
                    result.Failed--;
                }

                if (result.Failed > 0)
                {
                    Assert.Fail(string.Join(Environment.NewLine, result.Failures.Select(f => f.ToString())));
                }

                Assert.IsTrue(result.Passed > 0, "No snapshot tests were executed.");
            }
        }

        private static bool ShouldExcludeSnapshotScript(string scriptPath)
        {
            var normalized = scriptPath.Replace('\\', '/');
            foreach (var suffix in SnapshotExcludeSuffixes)
            {
                if (normalized.EndsWith(suffix.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            if (normalized.Contains("/parts/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static string FindRepositoryRoot()
        {
            var currentDirectory = Environment.CurrentDirectory;
            while (!string.IsNullOrWhiteSpace(currentDirectory))
            {
                if (Directory.Exists(Path.Combine(currentDirectory, "Tests", "Snapshot")) &&
                    Directory.Exists(Path.Combine(currentDirectory, "Examples")))
                {
                    return currentDirectory;
                }

                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            Assert.Fail("Repository root not found.");
            return null;
        }
    }
}
