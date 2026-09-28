using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;

namespace ShellScript.Testing
{
    public static class ShellScriptRunner
    {
        public const string DefaultPlatformName = "Unix-Bash";

        public static ScriptRunResult Run(
            string scriptPath,
            string platformName,
            string[] scriptArguments,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter)
        {
            scriptPath = Path.GetFullPath(scriptPath);
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("Source file not found.", scriptPath);
            }

            var platform = Platforms.GetPlatformByName(platformName);
            if (platform == null)
            {
                throw new InvalidOperationException($"Platform is not available: {platformName}");
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "ShellScript.Run." + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            var scriptBaseName = Path.GetFileNameWithoutExtension(scriptPath);
            var outputFile = Path.Combine(tempRoot, scriptBaseName + platform.ScriptExtension);
            var objDir = Path.Combine(tempRoot, "obj");

            try
            {
                var compiler = new Compiler();
                var flags = CompilerFlags.CreateDefault();
                flags = platform.ReviseFlags(flags);

                var compileResult = compiler.CompileFromSource(
                    errorWriter,
                    warningWriter,
                    logWriter,
                    scriptPath,
                    outputFile,
                    platformName,
                    flags);

                if (!compileResult.Successful)
                {
                    if (compileResult.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(compileResult.Exception).Throw();
                    }

                    throw new InvalidOperationException("Compilation failed.");
                }

                return PlatformScriptExecutor.Execute(outputFile, platform, Path.GetDirectoryName(scriptPath),
                    scriptArguments);
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempRoot))
                    {
                        Directory.Delete(tempRoot, true);
                    }
                }
                catch
                {
                    // Best-effort cleanup.
                }
            }
        }

        public static ScriptRunResult ExecuteBash(string scriptPath, string workingDirectory, string[] scriptArguments)
        {
            var argumentList = new System.Collections.Generic.List<string> {scriptPath};
            if (scriptArguments != null && scriptArguments.Length > 0)
            {
                argumentList.AddRange(scriptArguments);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = string.Join(" ",
                    argumentList.Select(QuoteProcessArgument)),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start bash.");
                }

                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();
                return new ScriptRunResult(process.ExitCode, stdout, stderr);
            }
        }

        private static string QuoteProcessArgument(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return "\"\"";
            }

            return "\"" + arg.Replace("\"", "\\\"") + "\"";
        }
    }
}
