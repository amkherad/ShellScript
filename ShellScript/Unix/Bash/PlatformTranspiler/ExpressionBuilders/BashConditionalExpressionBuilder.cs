using System;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashConditionalExpressionBuilder : BashDefaultExpressionBuilder
    {
        public new static BashConditionalExpressionBuilder Instance { get; } = new BashConditionalExpressionBuilder();


        public override string FormatExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.DataType.IsBoolean() || result.Template is LogicalEvaluationStatement)
            {
                if (result.Expression[0] != '[' && result.Expression[result.Expression.Length - 1] != ']')
                {
                    return $"[ {result.Expression} ]";
                }
            }

            return base.FormatExpression(p, result);
        }


        public override string FormatSubExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is LogicalEvaluationStatement)
            {
                return result.Expression;
            }

            return base.FormatSubExpression(p, result);
        }


        public override string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right,
            EvaluationStatement template)
        {
            return FormatLogicalExpression(p,
                left.DataType, left.Expression,
                op,
                right.DataType, right.Expression,
                template);
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.DataType.IsBoolean() && result.Template.ParentStatement is ConditionalBlockStatement)
            {
                return _formatBoolVariable(
                    base.FormatVariableAccessExpression(p, result)
                    );
            }

            return base.FormatVariableAccessExpression(p, result);
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            if (dataType.IsBoolean() && template.ParentStatement is ConditionalBlockStatement)
            {
                return _formatBoolVariable(
                    base.FormatVariableAccessExpression(p, dataType, expression, template)
                    );
            }

            return base.FormatVariableAccessExpression(p, dataType, expression, template);
        }

//        public override string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult result)
//        {
//            return base.FormatLogicalExpression(p, result);
//        }

        public override string FormatLogicalExpression(ExpressionBuilderParams p,
            DataTypes leftDataType, string left, IOperator op, DataTypes rightDataType, string right,
            EvaluationStatement template)
        {
            if (!(template is LogicalEvaluationStatement logicalEvaluationStatement))
                throw new InvalidOperationException();

            string opStr;

            if (leftDataType.IsString() || rightDataType.IsString())
            {
                return base.FormatLogicalExpression(p, leftDataType, left, op, rightDataType, right, template);
            }

            if (leftDataType.IsNumericOrFloat() || rightDataType.IsNumericOrFloat())
            {
                return base.FormatLogicalExpression(p, leftDataType, left, op, rightDataType, right, template);
            }

            switch (op)
            {
                case EqualOperator _:
                {
                    opStr = "-eq";
                    break;
                }
                case NotEqualOperator _:
                {
                    opStr = "-ne";
                    break;
                }
                case GreaterOperator _:
                {
                    opStr = "-gt";
                    break;
                }
                case GreaterEqualOperator _:
                {
                    opStr = "-ge";
                    break;
                }
                case LessOperator _:
                {
                    opStr = "-lt";
                    break;
                }
                case LessEqualOperator _:
                {
                    opStr = "-le";
                    break;
                }
                default:
                    opStr = op.ToString();
                    break;
            }

            left = _createBoolExpression(leftDataType, left, logicalEvaluationStatement.Left);
            right = _createBoolExpression(rightDataType, right, logicalEvaluationStatement.Right);

            return $"{left} {opStr} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string _formatBoolVariable(string exp)
        {
            return $"[ {exp} -ne 0 ]";
        }

        private string _createBoolExpression(DataTypes dataType, string exp, IStatement template)
        {
            if (template is ConstantValueStatement _)
            {
                return exp;
            }

            if (template is VariableAccessStatement _)
            {
                return _formatBoolVariable(exp);
            }

            if (template is LogicalEvaluationStatement)
            {
                if (exp[0] != '[' && exp[exp.Length - 1] != ']')
                {
                    return $"[ {exp} ]";
                }

                return exp;
            }

            if (dataType.IsBoolean())
            {
                if (exp[0] != '[' && exp[exp.Length - 1] != ']')
                {
                    return $"[ {exp} ]";
                }

                return exp;
            }

            return $"$(({exp}))";
        }

        public override ExpressionResult CreateExpression(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            var result = base.CreateExpression(p, statement);
            if (result.DataType != DataTypes.Boolean)
            {
                throw new TypeMismatchCompilerException(result.DataType, DataTypes.Boolean, statement.Info);
            }

            return result;
        }
    }
}