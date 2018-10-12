using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashStringConcatenationExpressionBuilder : BashDefaultExpressionBuilder
    {
        public new static BashStringConcatenationExpressionBuilder Instance { get; } =
            new BashStringConcatenationExpressionBuilder();

        public override string FormatSubExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.DataType == DataTypes.String)
            {
                return result.Expression;
            }

            return base.FormatSubExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    if (result.Template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement)
                    {
                        if (arithmeticEvaluationStatement.Operator is AdditionOperator)
                        {
                            if (p.FormatString)
                                return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                        }
                        else
                        {
                            throw new InvalidStatementStructureCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }
                    }

                    return base.FormatConstantExpression(p, result);
                }
            }

            return base.FormatConstantExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    if (template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement)
                    {
                        if (arithmeticEvaluationStatement.Operator is AdditionOperator)
                        {
                            if (p.FormatString)
                                return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                        }
                        else
                        {
                            throw new InvalidStatementStructureCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }
                    }

                    return base.FormatConstantExpression(p, dataType, expression, template);
                }
            }

            return base.FormatConstantExpression(p, dataType, expression, template);
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            var exp = base.FormatVariableAccessExpression(p, result);

            if (!result.DataType.IsString())
            {
                return exp;
            }
            
            if (result.Template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement &&
                arithmeticEvaluationStatement.Operator is AdditionOperator)
            {
                return exp;
            }

//            if (result.Template.ParentStatement is AssignmentStatement ||
//                result.Template.ParentStatement is VariableDefinitionStatement)
//            {
//                return $"\"{base.FormatVariableAccessExpression(p, result)}\"";
//            }

            if (exp[0] == '"' && exp[exp.Length - 1] == '"')
            {
                return exp;
            }

            return $"\"{exp}\"";
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            var exp = base.FormatVariableAccessExpression(p, dataType, expression, template);
            
            if (!dataType.IsString())
            {
                return exp;
            }
            
            if (template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement &&
                arithmeticEvaluationStatement.Operator is AdditionOperator)
            {
                return exp;
            }

//            if (template.ParentStatement is AssignmentStatement ||
//                template.ParentStatement is VariableDefinitionStatement)
//            {
//                return $"\"{base.FormatVariableAccessExpression(p, dataType, expression, template)}\"";
//            }

            if (exp[0] == '"' && exp[exp.Length - 1] == '"')
            {
                return exp;
            }

            return $"\"{exp}\"";
        }

        protected override ExpressionResult CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (p.UsageContext is IfElseStatement)
                    {
                        break;
                    }

                    switch (logicalEvaluationStatement.Operator)
                    {
                        case EqualOperator _:
                        case NotEqualOperator _:
                        {
                            var leftResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Left);
                            var rightResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Right);

                            if (leftResult.DataType.IsString() && rightResult.DataType.IsString())
                            {
                                HandleNotices(p, ref leftResult, ref rightResult);

                                p.NonInlinePartWriter.WriteLine(
                                    $"[ {leftResult.Expression} {logicalEvaluationStatement.Operator} {rightResult.Expression} ]");

                                string varName;

                                if (logicalEvaluationStatement.ParentStatement is AssignmentStatement ||
                                    logicalEvaluationStatement.ParentStatement is VariableDefinitionStatement)
                                {
                                    varName = UnixBashPlatform.LastStatusCodeStoreVariableName;
                                }
                                else
                                {
                                    varName = BashVariableDefinitionStatementTranspiler
                                        .WriteLastStatusCodeStoreVariableDefinition(p.Context, p.Scope,
                                            p.NonInlinePartWriter, "cmp_strings");
                                }

                                var template = new VariableAccessStatement(
                                    varName,
                                    logicalEvaluationStatement.Info
                                );

                                return new ExpressionResult(
                                    DataTypes.Boolean,
                                    $"${varName}",
                                    template,
                                    PinRequiredNotice
                                );
                            }

                            break;
                        }
                    }

                    break;
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case AdditionOperator _:
                        {
                            var leftResult = CreateExpressionRecursive(p, arithmeticEvaluationStatement.Left);
                            var rightResult = CreateExpressionRecursive(p, arithmeticEvaluationStatement.Right);

                            if (leftResult.DataType.IsString() || rightResult.DataType.IsString())
                            {
                                HandleNotices(p, ref leftResult, ref rightResult);

                                var leftExp = leftResult.Expression;
                                var rightExp = rightResult.Expression;

                                if (leftResult.Template is VariableAccessStatement)
                                {
                                    leftExp = FormatStringConcatenationVariableAccess(leftExp);
                                }

                                if (rightResult.Template is VariableAccessStatement)
                                {
                                    rightExp = FormatStringConcatenationVariableAccess(rightExp);
                                }


                                if (leftResult.DataType != DataTypes.String &&
                                    !(leftResult.Template is VariableAccessStatement))
                                {
                                    leftExp = $"$(({leftExp}))";
                                }

                                if (rightResult.DataType != DataTypes.String &&
                                    !(rightResult.Template is VariableAccessStatement))
                                {
                                    rightExp = $"$(({rightExp}))";
                                }

                                var concatExp = $"{leftExp}{rightExp}";

                                var newTemp = new ArithmeticEvaluationStatement(
                                    leftResult.Template,
                                    arithmeticEvaluationStatement.Operator,
                                    rightResult.Template,
                                    arithmeticEvaluationStatement.Info
                                );

                                leftResult.Template.ParentStatement = newTemp;
                                rightResult.Template.ParentStatement = newTemp;

                                return new ExpressionResult(
                                    DataTypes.String,
                                    concatExp,
                                    newTemp
                                );
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            return base.CreateExpressionRecursive(p, statement);
        }
    }
}