using System;
using System.IO;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashIfElseStatementTranspiler : IfElseStatementTranspilerBase
    {
        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is IfElseStatement ifElseStatement)) throw new InvalidOperationException();

            using (var ifWriter = new StringWriter())
            {
                WriteIf(context, scope, ifWriter, writer, metaWriter, ifElseStatement);

                writer.Write(ifWriter);
            }
        }

        public static void WriteIf(Context context, Scope scope, TextWriter writer, TextWriter nonInlinePartWriter,
            TextWriter metaWriter, IfElseStatement ifElseStatement)
        {
            var ifEscaped = false;
            var skipElse = false;

            var condition =
                EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, ifElseStatement.MainIf.Condition);

            if (StatementHelpers.IsAbsoluteBooleanValue(condition, out var isTrue))
            {
                if (isTrue)
                {
                    BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                        ifElseStatement.MainIf.Statement);

                    return;
                }

                ifEscaped = true;
            }
            else
            {
                writer.Write("if ");
                WriteCondition(context, scope, writer, metaWriter, nonInlinePartWriter, ifElseStatement, condition);
                writer.WriteLine();
                writer.WriteLine("then");
                BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                    ifElseStatement.MainIf.Statement);
            }

            if (ifElseStatement.ElseIfs != null)
            {
                foreach (var elseIf in ifElseStatement.ElseIfs)
                {
                    condition = EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, elseIf.Condition);

                    if (StatementHelpers.IsAbsoluteBooleanValue(condition, out isTrue))
                    {
                        if (isTrue)
                        {
                            if (!ifEscaped)
                            {
                                writer.WriteLine("else");
                            }

                            BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                                elseIf.Statement);

                            skipElse = true;
                            break;
                        }
                    }
                    else
                    {
                        if (ifEscaped)
                        {
                            writer.Write("if ");
                            ifEscaped = false;
                        }
                        else
                        {
                            writer.Write("elif ");
                        }

                        WriteCondition(context, scope, writer, metaWriter, nonInlinePartWriter, ifElseStatement,
                            condition);
                        writer.WriteLine();
                        writer.WriteLine("then");
                        BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                            elseIf.Statement);
                    }
                }
            }

            if (!skipElse && ifElseStatement.Else != null)
            {
                if (ifEscaped)
                {
                    BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                        ifElseStatement.Else);
                }
                else
                {
                    writer.WriteLine("else");
                    BashBlockStatementTranspiler.WriteBlockStatement(context, scope, writer, metaWriter,
                        ifElseStatement.Else);
                }
            }

            if (!ifEscaped)
            {
                writer.WriteLine("fi");
            }
        }

        public static void WriteCondition(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IfElseStatement ifElseStatement, EvaluationStatement statement)
        {
            var expressionBuilder = context.GetEvaluationTranspilerForStatement(statement);

            var result = expressionBuilder.GetConditionalExpression(context, scope, metaWriter, nonInlinePartWriter,
                ifElseStatement, statement);

            if (!result.TypeDescriptor.IsBoolean())
            {
                throw new InvalidStatementStructureCompilerException(statement, statement.Info);
            }

            writer.Write(result.Expression);
        }
    }
}