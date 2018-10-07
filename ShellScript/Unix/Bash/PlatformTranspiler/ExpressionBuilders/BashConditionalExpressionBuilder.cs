using System;
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


        public override string FormatExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            if (template is LogicalEvaluationStatement)
            {
                if (expression[0] != '[' && expression[expression.Length - 1] != ']')
                {
                    return $"[ {expression} ]";
                }
            }

            return base.FormatExpression(p, expression, template);
        }

        public override string FormatExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template)
        {
            if (dataType.IsBoolean() || template is LogicalEvaluationStatement)
            {
                if (expression[0] != '[' && expression[expression.Length - 1] != ']')
                {
                    return $"[ {expression} ]";
                }
            }

            return base.FormatExpression(p, dataType, expression, template);
        }

        public override string FormatSubExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template)
        {
            if (template is LogicalEvaluationStatement)
            {
                return expression;
            }

            return base.FormatSubExpression(p, dataType, expression, template);
        }

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

        private string _createBoolExpression(DataTypes dataType, string exp, IStatement statement)
        {
            if (statement is ConstantValueStatement _)
            {
                return exp;
            }

            if (statement is VariableAccessStatement _)
            {
                return $"[ {exp} -ne 0 ]";
            }

            if (statement is LogicalEvaluationStatement)
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

        public override (DataTypes, string, EvaluationStatement) CreateExpression(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            var (dataType, exp, template) = base.CreateExpression(p, statement);
            if (dataType != DataTypes.Boolean)
            {
                throw new TypeMismatchCompilerException(dataType, DataTypes.Boolean, statement.Info);
            }

            return (dataType, exp, template);
        }
    }
}