using System.IO;
using ShellScript.Core.Language;

namespace ShellScript.Core.Language.Compiler.PostProcessing
{
    public sealed class GeneratedCodePostProcessContext
    {
        public GeneratedCodePostProcessContext(
            string source,
            IPlatform platform,
            CompilerFlags flags,
            string outputFilePath,
            TextWriter warningWriter,
            TextWriter logWriter)
        {
            Source = source ?? "";
            Platform = platform;
            Flags = flags;
            OutputFilePath = outputFilePath;
            WarningWriter = warningWriter;
            LogWriter = logWriter;
        }

        public string Source { get; }

        public IPlatform Platform { get; }

        public CompilerFlags Flags { get; }

        public string OutputFilePath { get; }

        public TextWriter WarningWriter { get; }

        public TextWriter LogWriter { get; }

        public GeneratedCodePostProcessContext WithSource(string source) =>
            new GeneratedCodePostProcessContext(source, Platform, Flags, OutputFilePath, WarningWriter, LogWriter);
    }
}
