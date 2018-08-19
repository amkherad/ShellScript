using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language
{
    public interface IPlatform
    {
        ISdk[] Sdks { get; }
        
        string Name { get; }
    }
}