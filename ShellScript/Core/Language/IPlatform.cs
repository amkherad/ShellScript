using System;
using System.Collections.Generic;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.PostProcessing;
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

        /// <summary>Default script file extension for this target (e.g. .bash, .ps1, .cmd).</summary>
        string ScriptExtension { get; }
        
        ValueTuple<TypeDescriptor, string, string>[] CompilerConstants { get; }

        CompilerFlags ReviseFlags(CompilerFlags flags);

        string GetDefaultValue(DataTypes dataType);

        /// <summary>Platform-specific post-processors run on the merged generated script (formatting, etc.).</summary>
        IReadOnlyList<IGeneratedCodePostProcessor> GeneratedCodePostProcessors { get; }
    }
}