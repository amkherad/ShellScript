using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;

namespace ShellScript.Core.Language.Compiler
{
    public static class DeadBranchElimination
    {
        public static bool IsEnabled(Context context) => context.Flags.UseDeadBranchElimination;

        public static bool TryGetConstantBooleanCondition(Context context, Scope scope,
            EvaluationStatement condition, out bool isTrue)
        {
            isTrue = false;
            if (condition == null)
            {
                return false;
            }

            var folded = EvaluationStatementTranspilerBase.ProcessEvaluation(context, scope, condition);
            return StatementHelpers.IsAbsoluteBooleanValue(folded, out isTrue);
        }

        public static bool IsSwitchCaseDead(Context context, Scope scope, EvaluationStatement caseCondition)
        {
            if (!TryGetConstantBooleanCondition(context, scope, caseCondition, out var matches))
            {
                return false;
            }

            return !matches;
        }
    }
}
