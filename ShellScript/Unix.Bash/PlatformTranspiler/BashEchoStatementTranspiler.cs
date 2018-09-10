using System;
using System.IO;
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

            using (var inlineWriter = new StringWriter())
            {
                var isFirst = true;
                foreach (var stt in echoStatement.Parameters)
                {
                    if (!isFirst)
                    {
                        inlineWriter.Write(' ');
                    }

                    isFirst = false;

                    var transpiler = context.GetEvaluationTranspilerForStatement(stt);

                    transpiler.WriteInline(context, scope, inlineWriter, metaWriter, writer, stt);
                }

                writer.Write("echo ");
                writer.Write(inlineWriter.ToString());

                var device = scope.GetConfig(c => c.ExplicitEchoStream, context.Flags.ExplicitEchoStream);
                if (!string.IsNullOrWhiteSpace(device))
                {
                    writer.Write($" > {device}");
                }

                writer.WriteLine();
            }
        }
    }
}