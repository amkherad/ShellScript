using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;

namespace ShellScript.CommandLine
{
    public class RunCommand : ICommand
    {
        public const string DefaultPlatformName = "Unix-Bash";

        public string Name => "Run";

        public Dictionary<string, string> SwitchesHelp { get; } = new Dictionary<string, string>
        {
            {"platform", "Target platform name (default: Unix-Bash). Example: --platform=Unix-Bash"}
        };

        public int ProcessExitCode { get; private set; }

        public bool CanHandle(CommandContext command) => command.IsCommand("run");

        public ResultCodes Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            ProcessExitCode = 1;

            var inputFile = GetSourceFilePath(context);
            if (inputFile == null)
            {
                errorWriter.WriteLine("Source file is not specified.");
                return ResultCodes.Failure;
            }

            inputFile = Path.GetFullPath(inputFile);
            if (!File.Exists(inputFile))
            {
                errorWriter.WriteLine($"Source file not found: {inputFile}");
                return ResultCodes.Failure;
            }

            var platformName = context.GetSwitch("platform")?.Value ?? DefaultPlatformName;
            var platform = Platforms.GetPlatformByName(platformName);
            if (platform == null)
            {
                errorWriter.WriteLine($"Platform is not available: {platformName}");
                return ResultCodes.Failure;
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "ShellScript.Run." + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempRoot);

            var scriptBaseName = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(tempRoot, scriptBaseName + ".bash");
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
                    inputFile,
                    outputFile,
                    platformName,
                    flags);

                if (!compileResult.Successful)
                {
                    if (compileResult.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(compileResult.Exception).Throw();
                    }

                    return ResultCodes.Failure;
                }

                var scriptArgs = GetScriptArguments(context).ToArray();
                ProcessExitCode = RunBash(outputFile, Path.GetDirectoryName(inputFile), scriptArgs);
                return ProcessExitCode == 0 ? ResultCodes.Successful : ResultCodes.Failure;
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
                    // Best-effort cleanup of temp compile output.
                }
            }
        }

        private static int RunBash(string scriptPath, string workingDirectory, string[] scriptArgs)
        {
            var argumentList = new List<string> {scriptPath};
            if (scriptArgs != null && scriptArgs.Length > 0)
            {
                argumentList.AddRange(scriptArgs);
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = string.Join(" ",
                    argumentList.Select(QuoteProcessArgument)),
                UseShellExecute = false,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start bash.");
                }

                process.WaitForExit();
                return process.ExitCode;
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

        private static string GetSourceFilePath(CommandContext context)
        {
            var switchTokens = BuildSwitchTokenSet(context);

            for (var i = 1; i < context.Tokens.Length; i++)
            {
                var token = context.Tokens[i];
                if (switchTokens.Contains(token))
                {
                    continue;
                }

                if (token.StartsWith("-", StringComparison.Ordinal))
                {
                    continue;
                }

                return token;
            }

            return null;
        }

        private static IEnumerable<string> GetScriptArguments(CommandContext context)
        {
            var switchTokens = BuildSwitchTokenSet(context);
            var skippedSource = false;

            for (var i = 1; i < context.Tokens.Length; i++)
            {
                var token = context.Tokens[i];
                if (switchTokens.Contains(token) || token.StartsWith("-", StringComparison.Ordinal))
                {
                    continue;
                }

                if (!skippedSource)
                {
                    skippedSource = true;
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
