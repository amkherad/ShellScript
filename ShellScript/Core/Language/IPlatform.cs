using System;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language
{
    public interface IPlatform
    {
        IPlatformMetaInfoTranspiler MetaInfoWriter { get; }
        IPlatformStatementTranspiler[] Transpilers { get; }
        IApi Api { get; }
        
        string Name { get; }
        
        ValueTuple<DataTypes, string, string>[] CompilerConstants { get; }

        CompilerFlags ReviseFlags(CompilerFlags flags);
    }
}