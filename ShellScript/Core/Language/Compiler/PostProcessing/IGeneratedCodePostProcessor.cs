using ShellScript.Core.Language;

namespace ShellScript.Core.Language.Compiler.PostProcessing
{
    /// <summary>
    /// Transforms merged generated script text after transpilation (formatting, lint fixes, etc.).
    /// </summary>
    public interface IGeneratedCodePostProcessor
    {
        string Name { get; }

        bool IsEnabled(CompilerFlags flags, IPlatform platform);

        string Process(GeneratedCodePostProcessContext context);
    }
}
