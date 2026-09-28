using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Windows.Batch.PlatformTranspiler
{
    public class BatchWhileStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(WhileStatement);
        public override bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            var loop = statement as WhileStatement;
            if (loop == null || !loop.Condition.GetDataType(context, scope).IsBoolean())
            {
                message = "The while condition must be boolean.";
                return false;
            }
            return base.Validate(context, scope, statement, out message);
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement) => throw new NotSupportedException();

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            var loop = (WhileStatement) statement;
            var loopScope = scope.BeginNewScope(ScopeType.Block);

            if (DeadBranchElimination.IsEnabled(context) &&
                DeadBranchElimination.TryGetConstantBooleanCondition(context, loopScope, loop.Condition,
                    out var constantCondition))
            {
                if (!constantCondition)
                {
                    return;
                }

                writer.WriteLine("while :");
                writer.WriteLine("do");
                BatchBlockStatementTranspiler.WriteBlockStatement(
                    context, loopScope, writer, metaWriter, loop.Statement, ScopeType.Block, false);
                writer.WriteLine("done");
                scope.IncrementStatements();
                return;
            }

            var transpiler = context.GetEvaluationTranspilerForStatement(loop.Condition);
            writer.WriteLine("while");
            var condition = transpiler.GetConditionalExpression(
                context, loopScope, metaWriter, writer, loop, loop.Condition);
            writer.WriteLine(condition.Expression);
            writer.WriteLine("do");
            BatchBlockStatementTranspiler.WriteBlockStatement(
                context, loopScope, writer, metaWriter, loop.Statement, ScopeType.Block, false);
            writer.WriteLine("done");
            scope.IncrementStatements();
        }
    }
}
