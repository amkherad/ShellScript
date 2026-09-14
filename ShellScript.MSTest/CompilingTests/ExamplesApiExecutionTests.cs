using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class ExamplesApiExecutionTests
    {
        private const string SandboxVariable = "SHELLSCRIPT_EXAMPLE_ROOT";

        [TestMethod]
        public void AllApiExamplesCompileRunAndMatchExpectations()
        {
            var examplesRoot = FindExamplesApiRoot();
            var scripts = Directory.GetFiles(examplesRoot, "*.shellscript", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            Assert.IsTrue(scripts.Length > 0, "No API examples found under Examples/Api.");

            var platform = new UnixBashPlatform();
            var compiler = new Compiler();
            var tempRoot = Path.Combine(Path.GetTempPath(), "ShellScript.ApiExamples." + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            var failures = new List<string>();

            try
            {
                foreach (var script in scripts)
                {
                    var name = Path.GetFileNameWithoutExtension(script);
                    var expectations = ParseExpectations(File.ReadAllLines(script));
                    var outputFile = Path.Combine(tempRoot, name + ".bash");
                    var objDir = Path.Combine(tempRoot, "obj", name);
                    Directory.CreateDirectory(objDir);

                    try
                    {
                        using (var logWriter = new StringWriter())
                        {
                            compiler.CompileFromSource(script, objDir, outputFile, platform,
                                CreateExampleFlags(), logWriter, logWriter, logWriter);
                        }

                        ValidateBashSyntax(outputFile);

                        var sandbox = Path.Combine(tempRoot, "sandbox", name);
                        Directory.CreateDirectory(sandbox);

                        var (exitCode, stdout, stderr) = RunBashScript(outputFile, sandbox, expectations.Args);

                        if (exitCode != expectations.ExitCode)
                        {
                            failures.Add($"{name}: exit code {exitCode}, expected {expectations.ExitCode}." +
                                Environment.NewLine + stderr);
                            continue;
                        }

                        foreach (var line in expectations.StdoutContains)
                        {
                            if (!stdout.Contains(line))
                            {
                                failures.Add($"{name}: stdout missing expected line '{line}'." +
                                    Environment.NewLine + "stdout:" + Environment.NewLine + stdout);
                            }
                        }

                        foreach (var line in expectations.StderrContains)
                        {
                            if (!stderr.Contains(line))
                            {
                                failures.Add($"{name}: stderr missing expected line '{line}'." +
                                    Environment.NewLine + "stderr:" + Environment.NewLine + stderr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failures.Add($"{name}: {ex}");
                    }
                }
            }
            finally
            {
                try
                {
                    Directory.Delete(tempRoot, true);
                }
                catch
                {
                    // Best-effort cleanup.
                }
            }

            if (failures.Count > 0)
            {
                Assert.Fail(string.Join(Environment.NewLine + Environment.NewLine, failures));
            }
        }

        private static ExampleExpectations ParseExpectations(string[] lines)
        {
            var result = new ExampleExpectations();
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (!trimmed.StartsWith("// @", StringComparison.Ordinal))
                {
                    continue;
                }

                if (trimmed.StartsWith("// @expect-out ", StringComparison.Ordinal))
                {
                    result.StdoutContains.Add(trimmed.Substring("// @expect-out ".Length).Trim());
                }
                else if (trimmed.StartsWith("// @expect-err ", StringComparison.Ordinal))
                {
                    result.StderrContains.Add(trimmed.Substring("// @expect-err ".Length).Trim());
                }
                else if (trimmed.StartsWith("// @expect-exit ", StringComparison.Ordinal))
                {
                    result.ExitCode = int.Parse(trimmed.Substring("// @expect-exit ".Length).Trim());
                }
                else if (trimmed.StartsWith("// @args ", StringComparison.Ordinal))
                {
                    result.Args = SplitArgs(trimmed.Substring("// @args ".Length).Trim());
                }
            }

            return result;
        }

        private static string[] SplitArgs(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new string[0];
            }

            return Regex.Matches(value, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(match => match.Value.Trim('"'))
                .ToArray();
        }

        private static (int exitCode, string stdout, string stderr) RunBashScript(
            string scriptPath, string sandbox, string[] extraArgs)
        {
            var args = new List<string> {scriptPath};
            if (extraArgs != null && extraArgs.Length > 0)
            {
                args.AddRange(extraArgs);
            }

            var startInfo = new ProcessStartInfo("bash", string.Join(" ",
                args.Select(arg => "\"" + arg.Replace("\"", "\\\"") + "\"")))
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            startInfo.Environment[SandboxVariable] = sandbox;

            using (var process = Process.Start(startInfo))
            {
                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit(60000);
                return (process.ExitCode, stdout, stderr);
            }
        }

        private static CompilerFlags CreateExampleFlags()
        {
            var flags = CompilerFlags.CreateDefault();
            flags.WriteShellScriptVersion = false;
            flags.PreferRandomHelperVariableNames = false;
            flags.BindThirdPartyUtilitiesAtInit = true;
            return flags;
        }

        private static string FindExamplesApiRoot()
        {
            var currentDirectory = Environment.CurrentDirectory;
            while (!string.IsNullOrWhiteSpace(currentDirectory))
            {
                var candidate = Path.Combine(currentDirectory, "Examples", "Api");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            Assert.Fail("Examples/Api directory not found.");
            return null;
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
                {
                    throw new InvalidOperationException($"bash -n failed for {outputFile}: " +
                        standardOutput + standardError);
                }
            }
        }

        private sealed class ExampleExpectations
        {
            public int ExitCode { get; set; } = 0;
            public List<string> StdoutContains { get; } = new List<string>();
            public List<string> StderrContains { get; } = new List<string>();
            public string[] Args { get; set; } = new string[0];
        }
    }
}
