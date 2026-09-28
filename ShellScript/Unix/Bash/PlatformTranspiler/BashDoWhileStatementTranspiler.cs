using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashDoWhileStatementTranspiler : StatementTranspilerBase
    {
        public override Type StatementType => typeof(DoWhileStatement);
        public override bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public override bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            var loop = statement as DoWhileStatement;
            if (loop == null || !loop.Condition.GetDataType(context, scope).IsBoolean())
            {
                message = "The do-while condition must be boolean.";
                return false;
            }
            return base.Validate(context, scope, statement, out message);
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement) => throw new NotSupportedException();

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            var loop = (DoWhileStatement) statement;
            var loopScope = scope.BeginNewScope(ScopeType.Block);

            var keepLooping = false;
            var constantTail = DeadBranchElimination.IsEnabled(context) &&
                               DeadBranchElimination.TryGetConstantBooleanCondition(context, loopScope,
                                   loop.Condition, out keepLooping);

            writer.WriteLine("while :");
            writer.WriteLine("do");
            BashBlockStatementTranspiler.WriteBlockStatement(
                context, loopScope, writer, metaWriter, loop.Statement, ScopeType.Block, false);

            if (!constantTail)
            {
                var transpiler = context.GetEvaluationTranspilerForStatement(loop.Condition);
                var condition = transpiler.GetConditionalExpression(
                    context, loopScope, metaWriter, writer, loop, loop.Condition);
                writer.Write("if ! ");
                writer.Write(condition.Expression);
                writer.WriteLine("; then");
                writer.WriteLine("break");
                writer.WriteLine("fi");
            }
            else if (!keepLooping)
            {
                writer.WriteLine("break");
            }

            writer.WriteLine("done");
            scope.IncrementStatements();
        }
    }
}
