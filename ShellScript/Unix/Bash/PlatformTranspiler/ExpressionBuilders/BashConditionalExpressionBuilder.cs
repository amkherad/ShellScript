using System;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashConditionalExpressionBuilder : BashDefaultExpressionBuilder
    {
        public new static BashConditionalExpressionBuilder Instance { get; } = new BashConditionalExpressionBuilder();


        public override string FormatExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.TypeDescriptor.IsBoolean() || result.Template is LogicalEvaluationStatement)
            {
                if (result.Expression[0] != '[' && result.Expression[result.Expression.Length - 1] != ']')
                {
                    return $"[ {result.Expression} ]";
                }
            }

            return base.FormatExpression(p, result);
        }

        protected override string FormatEvaluationExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return result.Expression;
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
                left.TypeDescriptor, left.Expression,
                op,
                right.TypeDescriptor, right.Expression,
                template);
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.TypeDescriptor.IsBoolean() && result.Template.ParentStatement is ConditionalBlockStatement)
            {
                return _formatBoolVariable(
                    base.FormatVariableAccessExpression(p, result)
                );
            }

            return base.FormatVariableAccessExpression(p, result);
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
            string expression, EvaluationStatement template)
        {
            if (typeDescriptor.IsBoolean() && template.ParentStatement is ConditionalBlockStatement)
            {
                return _formatBoolVariable(
                    base.FormatVariableAccessExpression(p, typeDescriptor, expression, template)
                );
            }

            return base.FormatVariableAccessExpression(p, typeDescriptor, expression, template);
        }

//        public override string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult result)
//        {
//            return base.FormatLogicalExpression(p, result);
//        }

        public override string FormatLogicalExpression(ExpressionBuilderParams p,
            TypeDescriptor leftTypeDescriptor, string left, IOperator op, TypeDescriptor rightTypeDescriptor, string right,
            EvaluationStatement template)
        {
            if (!(template is LogicalEvaluationStatement logicalEvaluationStatement))
                throw new InvalidOperationException();

            string opStr;

            if (leftTypeDescriptor.IsString() || rightTypeDescriptor.IsString())
            {
                return base.FormatLogicalExpression(p, leftTypeDescriptor, left, op, rightTypeDescriptor, right, template);
            }

            if (leftTypeDescriptor.IsNumericOrFloat() || rightTypeDescriptor.IsNumericOrFloat())
            {
                return base.FormatLogicalExpression(p, leftTypeDescriptor, left, op, rightTypeDescriptor, right, template);
            }

            var operatorNeedsToEvaluate = false;

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
                    operatorNeedsToEvaluate = true;
                    break;
            }

            left = _createBoolExpression(leftTypeDescriptor, left, logicalEvaluationStatement.Left, operatorNeedsToEvaluate);
            right = _createBoolExpression(rightTypeDescriptor, right, logicalEvaluationStatement.Right,
                operatorNeedsToEvaluate);

            return $"{left} {opStr} {right}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string _formatBoolVariable(string exp)
        {
            return $"[ {exp} -ne 0 ]";
        }

        private string _createBoolExpression(TypeDescriptor typeDescriptor, string exp, IStatement template,
            bool operatorNeedsToEvaluate)
        {
            if (template is ConstantValueStatement _)
            {
                return exp;
            }

            if (template is VariableAccessStatement _)
            {
                return operatorNeedsToEvaluate
                    ? _formatBoolVariable(exp)
                    : exp;
            }

            if (template is LogicalEvaluationStatement)
            {
                if (exp[0] != '[' && exp[exp.Length - 1] != ']')
                {
                    return $"[ {exp} ]";
                }

                return exp;
            }

            if (typeDescriptor.IsBoolean())
            {
                if (exp[0] != '[' && exp[exp.Length - 1] != ']')
                {
                    return $"[ {exp} ]";
                }

                return exp;
            }

            if (exp[0] == '(' && exp[exp.Length - 1] == ')')
            {
                return $"$(({exp.Substring(1, exp.Length - 2)}))";
            }

            return $"$(({exp}))";
        }

        public override ExpressionResult CreateExpression(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            var result = base.CreateExpression(p, statement);
            
            if (result.IsEmptyResult)
            {
                throw new InvalidStatementStructureCompilerException(statement, statement.Info);
            }
            
            if (!result.TypeDescriptor.IsBoolean())
            {
                throw new TypeMismatchCompilerException(result.TypeDescriptor, TypeDescriptor.Boolean, statement.Info);
            }

            return result;
        }
    }
}