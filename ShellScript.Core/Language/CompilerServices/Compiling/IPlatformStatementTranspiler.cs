using System;

namespace ShellScript.Core.Language.CompilerServices.Compiling
{
    public interface IPlatformStatementTranspiler
    {
        Type StatementType { get; }
    }
}