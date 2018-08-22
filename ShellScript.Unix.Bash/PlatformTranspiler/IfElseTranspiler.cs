using System;
using ShellScript.Core.Language.CompilerServices.Compiling;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class IfElseTranspiler : IPlatformStatementTranspiler
    {
        public Type StatementType => typeof(IfElseStatement);
    }
}