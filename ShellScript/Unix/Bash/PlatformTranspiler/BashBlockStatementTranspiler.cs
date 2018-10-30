using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashBlockStatementTranspiler : BlockStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is BlockStatement block)) throw new InvalidOperationException();

            WriteBlockStatement(context, scope, writer, metaWriter, block, ScopeType.Block, null);
        }

        public static void WriteBlockStatement(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement, ScopeType scopeType)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            var subScope = scope.BeginNewScope(scopeType);
            
            if (statement is BlockStatement block)
            {
                foreach (var stt in block.Statements)
                {
                    Compiler.Transpile(context, subScope, stt, writer, metaWriter);

                    scope.IncrementStatements();
                }
            }
            else
            {
                Compiler.Transpile(context, subScope, statement, writer, metaWriter);

                scope.IncrementStatements();
            }
        }

        public static void WriteBlockStatement(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement, ScopeType scopeType, Type breakOnStatementType)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            var subScope = scope.BeginNewScope(scopeType);
            
            if (statement is BlockStatement block)
            {
                foreach (var stt in block.Statements)
                {
                    Compiler.Transpile(context, subScope, stt, writer, metaWriter);

                    if (stt.GetType() == breakOnStatementType)
                    {
                        break;
                    }
                    
                    scope.IncrementStatements();
                }
            }
            else
            {
                Compiler.Transpile(context, subScope, statement, writer, metaWriter);

                scope.IncrementStatements();
            }
        }
    }
}