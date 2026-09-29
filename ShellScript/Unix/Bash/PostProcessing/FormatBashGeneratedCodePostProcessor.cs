using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.PostProcessing;

namespace ShellScript.Unix.Bash.PostProcessing
{
    public sealed class FormatBashGeneratedCodePostProcessor : IGeneratedCodePostProcessor
    {
        public string Name => "bash-format";

        public bool IsEnabled(CompilerFlags flags, IPlatform platform) =>
            flags.FormatGeneratedCode && platform.Name == UnixBashPlatform.PlatformName;

        public string Process(GeneratedCodePostProcessContext context) =>
            BashGeneratedCodeFormatter.Format(context.Source, context.Flags.GeneratedCodeIndentColumns);
    }
}
