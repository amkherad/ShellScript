using ShellScript.Core.Language;
using ShellScript.Core.Language.Sdk;
using ShellScript.Windows.Batch.Sdk;

namespace ShellScript.ShellScript.Windows.Batch
{
    public class WindowsPlatform : IPlatform
    {
        static WindowsPlatform()
        {
            Platforms.AddPlatform(new WindowsPlatform());
        }

        public string Name => "Windows";
        
        public ISdk[] Sdks { get; } = {new WindowsBatchSdk()};
    }
}