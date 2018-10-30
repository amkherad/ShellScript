using System;
using System.IO;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashAssignmentStatementTranspiler : BashEvaluationStatementTranspiler
    {
        public override Type StatementType => typeof(AssignmentStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement)
        {
            return true;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter,
            IStatement statement)
        {
            if (!(statement is AssignmentStatement assignmentStatement)) throw new InvalidOperationException();

            WriteAssignment(context, scope, writer, metaWriter, nonInlinePartWriter, assignmentStatement);
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter nonInlinePartWriter,
            TextWriter metaWriter, IStatement statement)
        {
            if (!(statement is AssignmentStatement assignmentStatement)) throw new InvalidOperationException();

            using (var writer = new StringWriter())
            {
                WriteAssignment(context, scope, writer, metaWriter, nonInlinePartWriter, assignmentStatement);

                nonInlinePartWriter.Write(writer);
            }
            
            scope.IncrementStatements();
        }

        private static void WriteAssignment(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, AssignmentStatement assignmentStatement)
        {
            var target = assignmentStatement.LeftSide as VariableAccessStatement;
            if (target == null)
            {
                if (assignmentStatement.LeftSide == null)
                {
                    throw new InvalidStatementStructureCompilerException(assignmentStatement, assignmentStatement.Info);
                }

                throw new InvalidStatementStructureCompilerException(
                    "Only variables can be presented in the left side of an assignment",
                    assignmentStatement.LeftSide.Info);
            }

            var evaluation = assignmentStatement.RightSide;
            if (evaluation == null)
            {
                throw new InvalidStatementStructureCompilerException(
                    "Unknown right side of an assignment found.", assignmentStatement.RightSide.Info);
            }

            if (!scope.TryGetVariableInfo(target, out var varInfo))
            {
                throw new IdentifierNotFoundCompilerException(target);
            }

            var transpiler = context.GetEvaluationTranspilerForStatement(evaluation);

            var result =
                transpiler.GetExpression(context, scope, metaWriter, nonInlinePartWriter, null, evaluation);

            writer.Write($"{varInfo.AccessName}=");
            writer.WriteLine(result.Expression);
        }
    }
}