using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashForStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(ForStatement);
        public override bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            if (!(statement is ForStatement))
            {
                message = "Invalid for statement.";
                return false;
            }
            return base.Validate(context, scope, statement, out message);
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement) => throw new NotSupportedException();

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            var loop = (ForStatement) statement;
            var loopScope = scope.BeginNewScope(ScopeType.Block);
            if (loop.PreLoopAssignment != null)
                foreach (var initialization in loop.PreLoopAssignment)
                    Compiler.Transpile(context, loopScope, initialization, writer, metaWriter);

            if (loop.Condition == null)
                writer.WriteLine("while :");
            else
            {
                if (!loop.Condition.GetDataType(context, loopScope).IsBoolean())
                    throw new InvalidOperationException("The for condition must be boolean.");
                var transpiler = context.GetEvaluationTranspilerForStatement(loop.Condition);
                writer.WriteLine("while");
                var condition = transpiler.GetConditionalExpression(
                    context, loopScope, metaWriter, writer, loop, loop.Condition);
                writer.WriteLine(condition.Expression);
            }

            writer.WriteLine("do");
            BashBlockStatementTranspiler.WriteBlockStatement(
                context, loopScope, writer, metaWriter, loop.Statement, ScopeType.Block, false);
            if (loop.AfterLoopEvaluations != null)
                foreach (var evaluation in loop.AfterLoopEvaluations)
                {
                    var expression = evaluation as EvaluationStatement;
                    if (expression != null)
                    {
                        context.GetEvaluationTranspilerForStatement(expression).WriteInline(
                            context, loopScope, writer, metaWriter, writer, expression);
                        writer.WriteLine();
                    }
                    else
                        Compiler.Transpile(context, loopScope, evaluation, writer, metaWriter);
                }
            writer.WriteLine("done");
            scope.IncrementStatements();
        }
    }
}
