using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashIfElseStatementTranspiler : IfElseStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotImplementedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is IfElseStatement ifElseStatement)) throw new InvalidOperationException();

            var condition =
                EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, ifElseStatement.MainIf.Condition);

            if (StatementHelpers.IsAbsoluteValue(condition, out var isTrue))
            {
                var stt = ifElseStatement.MainIf.Statement;
                var transpiler = context.GetTranspilerForStatement(stt);
                transpiler.WriteBlock(context, scope, writer, metaWriter, stt);

                return;
            }
            
            writer.Write("if ");
            WriteCondition(context, scope, writer, metaWriter, writer, condition);
            writer.WriteLine();
            writer.WriteLine("then");
            BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                ifElseStatement.MainIf.Statement);

            if (ifElseStatement.ElseIfs != null)
            {
                foreach (var elseIf in ifElseStatement.ElseIfs)
                {
                    condition = EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, elseIf.Condition);

                    writer.Write("elif ");
                    WriteCondition(context, scope, writer, metaWriter, writer, elseIf.Condition);
                    writer.WriteLine();
                    writer.WriteLine("then");
                    BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                        elseIf.Statement);
                }
            }

            if (ifElseStatement.Else != null)
            {
                writer.WriteLine("else");
                BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                    ifElseStatement.Else);
            }

            writer.WriteLine("fi");
        }


        public static void WriteCondition(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            writer.WriteLine(statement.ToString());
        }
    }
}