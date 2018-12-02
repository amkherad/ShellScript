using System;
using System.IO;
using ShellScript.CommandLine;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;

namespace ShellScript.LSP
{
    public class Analyzer
    {
        public ResultCodes Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            CommandContext context,
            string sourceCode)
        {
            IPlatform platform;

            var osKind = Environment.OSVersion.Platform;
            if (osKind == PlatformID.Win32NT || osKind == PlatformID.Win32S || osKind == PlatformID.Win32Windows ||
                osKind == PlatformID.WinCE)
            {
                platform = null;
            }
            else
            {
                platform = new UnixBashPlatform();
            }

            var compiler = new Compiler();

            var ctx = compiler.CompileFromSource(
                platform,
                CompilerFlags.CreateDefault(),
                errorWriter,
                warningWriter,
                logWriter,
                TextWriter.Null,
                TextWriter.Null,
                sourceCode
            );

            return ResultCodes.Successful;
        }
    }
}