using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;

namespace ShellScript.Core.Language.Compiler
{
    public static class PinElimination
    {
        public static bool IsEnabled(ExpressionBuilderParams p) => p.Context.Flags.UsePinElimination;

        public static bool CanElideFloatingPointPin(ExpressionBuilderParams p, EvaluationStatement template,
            ExpressionResult left, ExpressionResult right)
        {
            if (!IsEnabled(p))
            {
                return false;
            }

            if (template is ArithmeticEvaluationStatement)
            {
                return IsSimpleOperand(left) && IsSimpleOperand(right);
            }

            if (template is LogicalEvaluationStatement)
            {
                return IsSimpleOperand(left) && IsSimpleOperand(right);
            }

            return false;
        }

        public static bool CanElidePinForAssignmentResult(ExpressionBuilderParams p, ExpressionResult assignmentResult)
        {
            if (!IsEnabled(p))
            {
                return false;
            }

            return assignmentResult.Template is VariableAccessStatement;
        }

        private static bool IsSimpleOperand(ExpressionResult result)
        {
            if (result.Template == null)
            {
                return false;
            }

            return result.Template is ConstantValueStatement || result.Template is VariableAccessStatement;
        }
    }
}
