using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Testing;
using LambdaExpression = System.Linq.Expressions.LambdaExpression;

namespace ShellScript.CommandLine
{
    public class CompileCommand : ICommand
    {
        public string Name => "Compile";

        public bool CanHandle(CommandContext command)
        {
            if (command.IsCommand("compile"))
            {
                return true;
            }

            return false;
        }

        public ResultCodes Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            CommandContext context)
        {
            var pathArgs = GetPathArguments(context).ToList();
            var platformName = context.GetSwitch("platform")?.Value;

            if (platformName == null && pathArgs.Count > 0)
            {
                var last = pathArgs[pathArgs.Count - 1];
                if (Platforms.GetPlatformByName(last) != null)
                {
                    platformName = last;
                    pathArgs.RemoveAt(pathArgs.Count - 1);
                }
            }

            if (platformName == null)
            {
                errorWriter.WriteLine(
                    "Platform is not specified. Pass Unix-Bash, Windows-PowerShell, or Windows-Batch as the last argument, or use --platform=Unix-Bash.");
                return ResultCodes.Failure;
            }

            var platform = Platforms.GetPlatformByName(platformName);
            if (platform == null)
            {
                errorWriter.WriteLine($"Platform is not available: {platformName}");
                return ResultCodes.Failure;
            }

            var compiler = new Compiler();
            var flags = CompilerFlags.CreateDefault();
            foreach (var sw in _switches)
            {
                _setFlag(context, flags, sw.Value.Item1, sw.Key);
            }

            flags = platform.ReviseFlags(flags);

            if (TryGetExplicitInputOutput(pathArgs, out var explicitInput, out var explicitOutput))
            {
                return CompileOne(compiler, explicitInput, explicitOutput, platformName, flags, outputWriter,
                    errorWriter, warningWriter, logWriter);
            }

            if (pathArgs.Count == 0)
            {
                var cwd = Environment.CurrentDirectory;
                pathArgs.Add(Path.Combine(cwd, "**", "*.shellscript"));
                outputWriter.WriteLine(
                    $"No paths given; discovering sources under {cwd} (**/*.shellscript).");
            }
            else if (pathArgs.All(p => p.IndexOf('*', StringComparison.Ordinal) < 0) && pathArgs.Count <= 3)
            {
                warningWriter.WriteLine(
                    "Your shell may have expanded a glob before the CLI saw it. Quote the pattern, e.g. " +
                    "shellscript compile 'Examples/**/*.shellscript' Unix-Bash");
            }

            var allMatches = SnapshotTester.ResolveScripts(pathArgs).ToArray();
            var scripts = allMatches.Where(ShellScriptSourceFilters.IsStandaloneEntry).ToArray();
            var skipped = allMatches.Length - scripts.Length;
            if (skipped > 0)
            {
                warningWriter.WriteLine(
                    $"Skipping {skipped} path(s) (bin/obj output, include parts/, API resources, or launcher stub).");
            }

            if (scripts.Length == 0)
            {
                errorWriter.WriteLine("No .shellscript files matched the given path(s).");
                return ResultCodes.Failure;
            }

            var failed = 0;
            foreach (var script in scripts)
            {
                var outputFile = GetOutputPathBesideSource(script, platform);
                var result = compiler.CompileFromSource(
                    errorWriter,
                    warningWriter,
                    logWriter,
                    script,
                    outputFile,
                    platformName,
                    flags);

                if (result.Successful)
                {
                    outputWriter.WriteLine($"Compiled: {script} -> {outputFile}");
                }
                else
                {
                    failed++;
                    if (result.Exception != null)
                    {
                        errorWriter.WriteLine($"{script}: {result.Exception.Message}");
                    }
                    else
                    {
                        errorWriter.WriteLine($"{script}: compilation failed.");
                    }
                }
            }

            if (failed > 0)
            {
                errorWriter.WriteLine($"Compilation finished with {failed} failure(s) out of {scripts.Length} file(s).");
                return ResultCodes.Failure;
            }

            outputWriter.WriteLine($"Compilation finished successfully ({scripts.Length} file(s)).");
            return ResultCodes.Successful;
        }

        private static ResultCodes CompileOne(
            Compiler compiler,
            string inputFile,
            string outputFile,
            string platformName,
            CompilerFlags flags,
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter)
        {
            if (!File.Exists(inputFile))
            {
                errorWriter.WriteLine($"Source file not found: {inputFile}");
                return ResultCodes.Failure;
            }

            var result = compiler.CompileFromSource(
                errorWriter,
                warningWriter,
                logWriter,
                inputFile,
                outputFile,
                platformName,
                flags);

            if (result.Successful)
            {
                outputWriter.WriteLine("Compilation finished successfully.");
                return ResultCodes.Successful;
            }

            var info = ExceptionDispatchInfo.Capture(result.Exception);
            info.Throw();
            return ResultCodes.Failure;
        }

        public static string GetOutputPathBesideSource(string scriptPath, IPlatform platform)
        {
            var directory = Path.GetDirectoryName(scriptPath) ?? string.Empty;
            var baseName = Path.GetFileNameWithoutExtension(scriptPath);
            return Path.Combine(directory, baseName + platform.ScriptExtension);
        }

        private static bool TryGetExplicitInputOutput(
            List<string> pathArgs,
            out string inputFile,
            out string outputFile)
        {
            inputFile = null;
            outputFile = null;

            if (pathArgs.Count != 2)
            {
                return false;
            }

            if (!pathArgs[0].EndsWith(".shellscript", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (pathArgs[1].EndsWith(".shellscript", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (pathArgs[0].IndexOf('*', StringComparison.Ordinal) >= 0 ||
                pathArgs[1].IndexOf('*', StringComparison.Ordinal) >= 0)
            {
                return false;
            }

            inputFile = pathArgs[0];
            outputFile = pathArgs[1];
            return true;
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

        private void _setFlag(CommandContext context, CompilerFlags flags,
            Expression<Func<CompilerFlags, object>> prop, string switchName)
        {
            Switch s;

            if ((s = context.GetSwitch(switchName)) != null)
            {
                s.AssertValue();

                var lambda = prop as LambdaExpression;
                if (lambda == null)
                {
                    throw new InvalidOperationException();
                }

                var memberExpr = GetMemberExpression(lambda.Body);
                if (memberExpr == null)
                {
                    throw new InvalidOperationException();
                }

                var propName = memberExpr.Member.Name;

                var propInfo = flags.GetType().GetProperty(propName);

                if (propInfo.PropertyType == typeof(bool))
                {
                    StatementHelpers.TryParseBooleanFromString(s.Value, out var value);
                    propInfo.SetValue(flags, value);
                }
                else
                {
                    propInfo.SetValue(flags, Convert.ChangeType(s.Value, propInfo.PropertyType));
                }
            }
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is MemberExpression member)
            {
                return member;
            }

            if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                return unary.Operand as MemberExpression;
            }

            return null;
        }

        private static Dictionary<string, (Expression<Func<CompilerFlags, object>>, string)> _switches =
            new Dictionary<string, (Expression<Func<CompilerFlags, object>>, string)>
            {
                {
                    "echo-dev",
                    (
                        x => x.ExplicitEchoStream,
                        "Specifies the explicit device for standard output, default is /dev/tty."
                    )
                },

                {
                    "default-echo-dev",
                    (
                        x => x.DefaultExplicitEchoStream,
                        "Specifies the default explicit device for standard output, if not changed, /dev/tty will be used."
                    )
                },

                {
                    "use-comment",
                    (
                        x => x.UseComments,
                        "Determines whether meta-comments should be used in output code."
                    )
                },

                {
                    "use-third-party-utilities",
                    (
                        x => x.UseThirdPartyUtilities,
                        "When false, generated code uses pure-shell fallbacks instead of awk/bc/python."
                    )
                },

                {
                    "bind-utilities-at-init",
                    (
                        x => x.BindThirdPartyUtilitiesAtInit,
                        "When true, utility choice is resolved once in the script prologue."
                    )
                },

                {
                    "disabled-utilities",
                    (
                        x => x.DisabledThirdPartyUtilities,
                        "Comma-separated utility names to skip (awk, bc, python)."
                    )
                },

                {
                    "utility-order",
                    (
                        x => x.ThirdPartyUtilityOrder,
                        "Preferred third-party utility order (default: awk,bc,python)."
                    )
                },

                {
                    "post-process-generated-code",
                    (
                        x => x.PostProcessGeneratedCode,
                        "When false, skip all generated-code post-processors."
                    )
                },

                {
                    "format-generated-code",
                    (
                        x => x.FormatGeneratedCode,
                        "Format generated script output (Unix-Bash: built-in indent formatter)."
                    )
                },

                {
                    "generated-code-indent",
                    (
                        x => x.GeneratedCodeIndentColumns,
                        "Indent column count for format-generated-code (default 2)."
                    )
                },
            };

        public Dictionary<string, string> SwitchesHelp { get; } = CreateSwitchesHelp();

        private static Dictionary<string, string> CreateSwitchesHelp()
        {
            var help = _switches.ToDictionary(kv => kv.Key, kv => kv.Value.Item2);
            help["platform"] = "Target platform (Unix-Bash, Windows-PowerShell, Windows-Batch).";
            return help;
        }
    }
}
