using System;
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

            return base.FormatExpression(p, expression, template);
        }

        public override string FormatSubExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            if (template is LogicalEvaluationStatement)
            {
                return expression;
            }

            return base.FormatSubExpression(p, expression, template);
        }

        public override string FormatLogicalExpression(ExpressionBuilderParams p,
            DataTypes leftDataType, string left, IOperator op, DataTypes rightDataType, string right,
            EvaluationStatement template)
        {
            if (!(template is LogicalEvaluationStatement logicalEvaluationStatement))
                throw new InvalidOperationException();

            string opStr;

            if (!leftDataType.IsString() && !rightDataType.IsString())
            {
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
            }
            else
            {
                opStr = op.ToString();
            }

            left = _createStringExpression(left, logicalEvaluationStatement.Left);
            right = _createStringExpression(right, logicalEvaluationStatement.Right);

            return $"{left} {opStr} {right}";
        }

        private string _createStringExpression(string exp, IStatement statement)
        {
            if (statement is ConstantValueStatement _)
            {
                return exp;
            }

            if (statement is VariableAccessStatement _)
            {
                return exp;
            }

            if (statement is LogicalEvaluationStatement)
            {
                if (exp[0] != '[' && exp[exp.Length - 1] != ']')
                {
                    return $"[ {exp} ]";
                }
                else
                {
                    return exp;
                }
            }

            return $"$(({exp}))";
        }
    }
}