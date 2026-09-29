using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Language;

namespace ShellScript.Core.Language.Compiler.PostProcessing
{
    public static class GeneratedCodePostProcessorPipeline
    {
        public static string Apply(
            string source,
            IPlatform platform,
            CompilerFlags flags,
            string outputFilePath,
            TextWriter warningWriter,
            TextWriter logWriter)
        {
            if (!flags.PostProcessGeneratedCode)
            {
                return source;
            }

            var processors = CollectProcessors(platform);
            if (processors.Count == 0)
            {
                return source;
            }

            var context = new GeneratedCodePostProcessContext(source, platform, flags, outputFilePath, warningWriter,
                logWriter);

            foreach (var processor in processors)
            {
                if (!processor.IsEnabled(flags, platform))
                {
                    continue;
                }

                logWriter?.WriteLine($"Post-processing: {processor.Name}");
                var next = processor.Process(context);
                context = context.WithSource(next ?? context.Source);
            }

            return context.Source;
        }

        private static List<IGeneratedCodePostProcessor> CollectProcessors(IPlatform platform)
        {
            var list = new List<IGeneratedCodePostProcessor>();
            if (platform.GeneratedCodePostProcessors != null)
            {
                list.AddRange(platform.GeneratedCodePostProcessors);
            }

            return list;
        }
    }
}
