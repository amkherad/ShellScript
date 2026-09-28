using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ShellScript.Core.Language;

namespace ShellScript.Testing
{
    public static class PlatformScriptExecutor
    {
        public static ScriptRunResult Execute(string scriptPath, IPlatform platform, string workingDirectory,
            string[] scriptArguments)
        {
            switch (platform.Name)
            {
                case "Windows-PowerShell":
                    return ExecuteProcess(ResolveExecutable("pwsh", "powershell"), "-File", scriptPath, workingDirectory,
                        scriptArguments);
                case "Windows-Batch":
                    return ExecuteProcess(ResolveExecutable("cmd", "cmd.exe"), "/c", scriptPath, workingDirectory,
                        scriptArguments);
                default:
                    return ShellScriptRunner.ExecuteBash(scriptPath, workingDirectory, scriptArguments);
            }
        }

        private static string ResolveExecutable(string primary, string fallback)
        {
            foreach (var name in new[] {primary, fallback})
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var pathEnv = Environment.GetEnvironmentVariable("PATH");
                if (pathEnv == null)
                {
                    return name;
                }

                foreach (var dir in pathEnv.Split(Path.PathSeparator))
                {
                    if (string.IsNullOrWhiteSpace(dir))
                    {
                        continue;
                    }

                    var candidate = Path.Combine(dir.Trim(), name);
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            return primary;
        }

        private static ScriptRunResult ExecuteProcess(string executable, string leadArgument, string scriptPath,
            string workingDirectory, string[] scriptArguments)
        {
            var argumentList = new List<string> {leadArgument, scriptPath};
            if (scriptArguments != null && scriptArguments.Length > 0)
            {
                argumentList.AddRange(scriptArguments);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = string.Join(" ", argumentList.Select(QuoteProcessArgument)),
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
                    throw new InvalidOperationException($"Failed to start {executable}.");
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
