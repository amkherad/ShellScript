using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashReturnStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(ReturnStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return false;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter,
            IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is ReturnStatement returnStatement)) throw new InvalidOperationException();

            var result = returnStatement.Result;

            if (result != null)
            {
                var transpiler = context.GetEvaluationTranspilerForStatement(result);
                var (dataType, expression, template) = transpiler.GetInline(context, scope, metaWriter, writer, null, result);

                writer.Write("echo ");
                writer.Write(expression);
                writer.WriteLine();
            }

            writer.Write("return ");
            writer.Write(context.Flags.SuccessStatusCode);
            writer.WriteLine();
        }
    }
}