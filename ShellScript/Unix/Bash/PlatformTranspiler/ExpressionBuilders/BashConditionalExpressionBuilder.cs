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
                var exp = result.Expression;
                if (exp.Contains("&&", StringComparison.Ordinal) || exp.Contains("||", StringComparison.Ordinal))
                {
                    return exp;
                }

                if (_isBracketTest(exp))
                {
                    return exp;
                }

                if (result.Template is FunctionCallStatement)
                {
                    return $"[ {exp} -ne 0 ]";
                }

                return $"[ {exp} ]";
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
            if (result.TypeDescriptor.IsBoolean())
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
            if (typeDescriptor.IsBoolean())
            {
                return _formatBoolVariable(
                    base.FormatVariableAccessExpression(p, typeDescriptor, expression, template)
                );
            }

            return base.FormatVariableAccessExpression(p, typeDescriptor, expression, template);
        }

        protected override ExpressionResult CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            if (statement is LogicalEvaluationStatement logical && logical.Operator is NotOperator)
            {
                var inner = base.CreateExpressionRecursive(p, logical.Right);
                if (!inner.TypeDescriptor.IsBoolean())
                {
                    throw new InvalidStatementCompilerException(logical, logical.Info);
                }

                var exp = inner.Expression.Trim();
                const string notEqualZeroSuffix = "-ne 0";
                if (exp.StartsWith("[[", StringComparison.Ordinal) && exp.EndsWith("]]", StringComparison.Ordinal))
                {
                    var core = exp.Substring(2, exp.Length - 4).Trim();
                    return new ExpressionResult(TypeDescriptor.Boolean, $"[[ ! {core} ]]", logical);
                }

                if (exp.StartsWith("[") && exp.EndsWith("]"))
                {
                    var core = exp.Substring(1, exp.Length - 2).Trim();
                    if (core.EndsWith(notEqualZeroSuffix, StringComparison.Ordinal))
                    {
                        var varPart = core.Substring(0, core.Length - notEqualZeroSuffix.Length).TrimEnd();
                        return new ExpressionResult(TypeDescriptor.Boolean, $"[ {varPart} -eq 0 ]", logical);
                    }

                    if (core.StartsWith("-z ", StringComparison.Ordinal))
                    {
                        return new ExpressionResult(TypeDescriptor.Boolean,
                            $"[ -n {core.Substring(3).Trim()} ]", logical);
                    }

                    if (core.StartsWith("-n ", StringComparison.Ordinal))
                    {
                        return new ExpressionResult(TypeDescriptor.Boolean,
                            $"[ -z {core.Substring(3).Trim()} ]", logical);
                    }

                    return new ExpressionResult(TypeDescriptor.Boolean, $"[ ! {core} ]", logical);
                }

                if (logical.Right is VariableAccessStatement)
                {
                    var varPart = exp;
                    if (varPart.StartsWith("$", StringComparison.Ordinal))
                    {
                        varPart = varPart.Substring(1).Trim('{', '}');
                    }

                    return new ExpressionResult(TypeDescriptor.Boolean, $"[ {varPart} -eq 0 ]", logical);
                }

                return new ExpressionResult(TypeDescriptor.Boolean, $"[ ! {exp} ]", logical);
            }

            return base.CreateExpressionRecursive(p, statement);
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

            if (op is LogicalAndOperator || op is LogicalOrOperator)
            {
                left = _asBashCondition(leftTypeDescriptor, left, logicalEvaluationStatement.Left);
                right = _asBashCondition(rightTypeDescriptor, right, logicalEvaluationStatement.Right);
                return $"{left} {op} {right}";
            }

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

        private static bool _isBracketTest(string exp)
        {
            if (string.IsNullOrEmpty(exp))
            {
                return false;
            }

            if (exp.StartsWith("[[", StringComparison.Ordinal) && exp.EndsWith("]]", StringComparison.Ordinal))
            {
                return true;
            }

            return exp[0] == '[' && exp[exp.Length - 1] == ']';
        }

        private string _asBashCondition(TypeDescriptor typeDescriptor, string exp, IStatement template)
        {
            if (_isBracketTest(exp))
            {
                return exp;
            }

            if (template is ConstantValueStatement)
            {
                return exp;
            }

            if (template is VariableAccessStatement && typeDescriptor.IsBoolean())
            {
                return _formatBoolVariable(exp);
            }

            if (typeDescriptor.IsBoolean())
            {
                if (template is LogicalEvaluationStatement || template is FunctionCallStatement)
                {
                    return $"[ {exp} ]";
                }

                return $"[ {exp} -ne 0 ]";
            }

            return exp;
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
                if (_isBracketTest(exp))
                {
                    return exp;
                }

                if (template is LogicalEvaluationStatement || template is FunctionCallStatement)
                {
                    return $"[ {exp} ]";
                }

                return $"[ {exp} -ne 0 ]";
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