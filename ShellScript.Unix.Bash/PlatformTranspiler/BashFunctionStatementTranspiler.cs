using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashFunctionStatementTranspiler : FunctionStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement)
        {
            if (!(statement is FunctionStatement funcDefStt)) throw new InvalidOperationException();

            writer.WriteLine($"function {funcDefStt.Name}() {{");
            
            writer.WriteLine(':');
            
            writer.WriteLine("}");
        }
    }
}