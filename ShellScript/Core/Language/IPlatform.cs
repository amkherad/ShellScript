using System;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language
{
    public interface IPlatform
    {
        IPlatformMetaInfoTranspiler MetaInfoWriter { get; }
        IPlatformStatementTranspiler[] Transpilers { get; }
        IApi Api { get; }
        
        string Name { get; }
        
        ValueTuple<TypeDescriptor, string, string>[] CompilerConstants { get; }

        CompilerFlags ReviseFlags(CompilerFlags flags);

        string GetDefaultValue(DataTypes dataType);
    }
}