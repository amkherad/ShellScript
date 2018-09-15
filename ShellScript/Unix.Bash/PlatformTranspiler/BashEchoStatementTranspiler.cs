using System;
using System.IO;
using System.Text;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashEchoStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(EchoStatement);

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
            if (!(statement is EchoStatement echoStatement)) throw new InvalidOperationException();

            var paramExp = new StringBuilder();
            var isFirst = true;
            foreach (var stt in echoStatement.Parameters)
            {
                if (!isFirst)
                {
                    paramExp.Append(' ');
                }

                isFirst = false;

                var transpiler = context.GetEvaluationTranspilerForStatement(stt);

                var (dataType, expression) = transpiler.GetInline(context, scope, metaWriter, writer, null, stt);
                paramExp.Append(expression);
            }

            writer.Write("echo ");
            writer.Write(paramExp.ToString());

            var device = scope.GetConfig(c => c.ExplicitEchoStream, context.Flags.ExplicitEchoStream);
            if (!string.IsNullOrWhiteSpace(device))
            {
                writer.Write($" > {device}");
            }

            writer.WriteLine();
        }
    }
}