using System.Collections.Generic;

namespace ShellScript.Core.Language.Sdk
{
    public interface ISdkClass : ISdkObject
    {
        IEnumerable<ISdkVariable> Variables { get; }
        
        IEnumerable<ISdkFunc> Functions { get; }
    }
}