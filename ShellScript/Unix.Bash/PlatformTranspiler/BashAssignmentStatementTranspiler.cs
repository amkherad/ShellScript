using System;
using System.IO;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashAssignmentStatementTranspiler : StatementTranspilerBase
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

        public override void WriteBlock(Context context, Scope scope, TextWriter nonInlinePartWriter, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is AssignmentStatement assignmentStatement)) throw new InvalidOperationException();

            using (var writer = new StringWriter())
            {
                WriteAssignment(context, scope, writer, metaWriter, nonInlinePartWriter, assignmentStatement);
                
                writer.WriteLine();

                nonInlinePartWriter.Write(writer);
            }
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
                    "Only variables can be presented in the left side of an assignment", assignmentStatement.LeftSide.Info);
            }

            var evaluation = assignmentStatement.RightSide as EvaluationStatement;
            if (evaluation == null)
            {
                throw new InvalidStatementStructureCompilerException(
                    "Unknown right side of an assignment found.", assignmentStatement.RightSide.Info);
            }

            if (!scope.TryGetVariableInfo(target.VariableName, out var varInfo))
            {
                throw new IdentifierNotFoundCompilerException(target.VariableName, target.Info);
            }
            
            writer.Write($"{varInfo.AccessName}=");

            var transpiler = context.GetEvaluationTranspilerForStatement(evaluation);

            transpiler.WriteInline(context, scope, writer, metaWriter, nonInlinePartWriter, evaluation);
        }
    }
}