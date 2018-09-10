using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashFunctionStatementTranspiler : FunctionStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is FunctionStatement funcDefStt)) throw new InvalidOperationException();

            if (scope.IsIdentifierExists(funcDefStt.Name))
            {
                throw new IdentifierNameExistsCompilerException(funcDefStt.Name, funcDefStt.Info);
            }
            
            writer.WriteLine($"function {funcDefStt.Name}() {{");

            var funcScope = scope.BeginNewScope();
            IStatement inlinedStatement = null;

            funcScope.SetConfig(c => c.ExplicitEchoStream, context.Flags.DefaultExplicitEchoStream);

            if (IsEmptyBody(funcDefStt.Block))
            {
                writer.WriteLine(':');
            }
            else
            {
                var transpiler = context.GetTranspilerForStatement(funcDefStt.Block);
                
                transpiler.WriteBlock(context, funcScope, writer, metaWriter, funcDefStt.Block);

                TryGetInlinedStatement(context, funcScope, funcDefStt, out inlinedStatement);
            }

            var func = new FunctionInfo(funcDefStt.DataType, funcDefStt.Name, funcDefStt.IsParams,
                funcDefStt.Parameters, null, inlinedStatement);
            
            scope.ReserveNewFunction(funcDefStt.Name, func);

            writer.WriteLine("}");
        }
    }
}