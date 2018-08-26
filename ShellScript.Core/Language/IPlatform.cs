using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language
{
    public interface IPlatform
    {
        IPlatformMetaInfoTranspiler MetaInfoWriter { get; }
        IPlatformStatementTranspiler[] Transpilers { get; }
        ISdk Sdk { get; }
        
        string Name { get; }
        
        string[] CompilerConstants { get; }
    }
}