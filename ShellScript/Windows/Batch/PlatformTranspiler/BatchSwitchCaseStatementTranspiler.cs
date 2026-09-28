using System;
using System.IO;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Windows.Batch.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.Windows.Batch.PlatformTranspiler
{
    public class BatchSwitchCaseStatementTranspiler : IPlatformStatementTranspiler
    {
        public Type StatementType => typeof(SwitchCaseStatement);

        public bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public bool Validate(Context context, Scope scope, IStatement statement, out string message)
        {
            message = null;
            return statement is SwitchCaseStatement;
        }

        public void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement) =>
            throw new NotSupportedException();

        public void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is SwitchCaseStatement switchStatement))
            {
                throw new InvalidOperationException();
            }

            var blockScope = scope.BeginNewScope(ScopeType.Block);
            var target = EvaluationStatementTranspilerBase.ProcessEvaluation(context, blockScope,
                switchStatement.SwitchTarget);
            var targetExpression = BatchEvaluationStatementTranspiler.CreateBatchExpression(
                new ExpressionBuilderParams(context, blockScope, metaWriter, writer, switchStatement), target);

            writer.Write("case ");
            writer.Write(targetExpression.Expression);
            writer.WriteLine(" in");

            if (switchStatement.Cases != null)
            {
                foreach (var caseBlock in switchStatement.Cases)
                {
                    var caseCondition = EvaluationStatementTranspilerBase.ProcessEvaluation(context, blockScope,
                        caseBlock.Condition);
                    if (DeadBranchElimination.IsEnabled(context) &&
                        DeadBranchElimination.IsSwitchCaseDead(context, blockScope, caseCondition))
                    {
                        continue;
                    }

                    if (!TryGetCasePattern(caseCondition, out var pattern))
                    {
                        throw new InvalidOperationException("Invalid switch case condition.");
                    }

                    writer.WriteLine($"{pattern})");
                    BatchBlockStatementTranspiler.WriteBlockStatement(context, blockScope, writer, metaWriter,
                        caseBlock.Statement, ScopeType.Block, true);
                    writer.WriteLine(";;");
                }
            }

            if (switchStatement.DefaultCase != null)
            {
                writer.WriteLine("*)");
                BatchBlockStatementTranspiler.WriteBlockStatement(context, blockScope, writer, metaWriter,
                    switchStatement.DefaultCase, ScopeType.Block, true);
                writer.WriteLine(";;");
            }

            writer.WriteLine("esac");
            scope.IncrementStatements();
        }

        private static bool TryGetCasePattern(EvaluationStatement condition, out string pattern)
        {
            pattern = null;
            if (!(condition is LogicalEvaluationStatement logical) || !(logical.Operator is EqualOperator))
            {
                return false;
            }

            if (logical.Right is ConstantValueStatement constant)
            {
                if (constant.IsString())
                {
                    pattern = constant.Value.Trim('"');
                    return true;
                }

                pattern = constant.Value;
                return true;
            }

            return false;
        }
    }
}
