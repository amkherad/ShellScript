using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashBlockStatementTranspiler : BlockStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, IStatement statement)
        {
            if (!(statement is BlockStatement block)) throw new InvalidOperationException();

            WriteBlockStatement(context, scope, writer, metaWriter, block);
        }
        
        public static void WriteBlockStatement(Context context, Scope scope, TextWriter writer, TextWriter metaWriter, IStatement statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            
            if (statement is BlockStatement block)
            {
                foreach (var stt in block.Statements)
                {
                    Compiler.Transpile(context, scope, stt, writer, metaWriter);
                }
            }
            else
            {
                Compiler.Transpile(context, scope, statement, writer, metaWriter);
            }
        }
    }
}