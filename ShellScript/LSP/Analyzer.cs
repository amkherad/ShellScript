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

            using (var file = new FileStream(sourceCode, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(file))
            {
                var ctx = Compiler.CompileFromSource(platform,
                    CompilerFlags.CreateDefault(),
                    errorWriter,
                    warningWriter,
                    logWriter,
                    TextWriter.Null,
                    TextWriter.Null,
                    reader,
                    sourceCode
                );
            }

            return ResultCodes.Successful;
        }
    }
}