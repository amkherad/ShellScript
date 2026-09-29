using System;
using System.IO;
using System.Text;
using ShellScript.Core;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

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
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is EchoStatement echoStatement)) throw new InvalidOperationException();

            var paramExp = new StringBuilder();
            paramExp.Append("echo");
            foreach (var stt in echoStatement.Parameters)
            {
                paramExp.Append(' ');

                var transpiler = context.GetEvaluationTranspilerForStatement(stt);

                var result = transpiler.GetExpression(context, scope, metaWriter, writer, null, stt);
                paramExp.Append(QuoteEchoArgument(result.Expression));
            }

            writer.Write(paramExp.ToString());

            var device = scope.GetConfig(c => c.ExplicitEchoStream, context.Flags.ExplicitEchoStream);
            if (!string.IsNullOrWhiteSpace(device))
            {
                writer.Write($" > {device}");
            }

            writer.WriteLine();
            
            scope.IncrementStatements();
        }

        /// <summary>
        /// Unquoted ANSI reset sequences (${CReset} → ESC[0m) make bash treat [0m as a redirect target
        /// and create a junk file in the working directory.
        /// </summary>
        private static string QuoteEchoArgument(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                return "''";
            }

            return StringHelpers.EnQuote(expression);
        }
    }
}
