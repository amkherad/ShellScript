using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language
{
    public interface IPlatform
    {
        IPlatformStatementTranspiler[] Transpilers { get; }
        ISdk Sdk { get; }
        
        string Name { get; }
    }
}