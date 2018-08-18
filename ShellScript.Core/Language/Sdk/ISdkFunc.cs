using System.Collections.Generic;

namespace ShellScript.Core.Language.Sdk
{
    public interface ISdkFunc : ISdkObject
    {
        bool AllowDynamicParams { get; }
        
        IEnumerable<ISdkParameter> Parameters { get; }
    }
}