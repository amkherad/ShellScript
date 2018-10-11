using System.IO;
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
                    if (p.FormatString)
                        return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                    
                    return base.FormatConstantExpression(p, result);
                }
            }

            return base.FormatConstantExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    if (p.FormatString)
                        return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                    
                    return base.FormatConstantExpression(p, expression, template);
                }
            }

            return base.FormatConstantExpression(p, expression, template);
        }

        protected override ExpressionResult CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    if (constantValueStatement.IsString())
                    {
                        return new ExpressionResult(
                            DataTypes.String,
                            p.FormatString
                                ? BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false)
                                : BashTranspilerHelpers.StandardizeString(constantValueStatement.Value, true),
                            constantValueStatement
                        );
                    }

                    return new ExpressionResult(
                        constantValueStatement.DataType,
                        constantValueStatement.Value,
                        constantValueStatement
                    );
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case AdditionOperator _:
                        {
                            using (var nonInlinePartWriterPinned = new StringWriter())
                            {
                                var np = new ExpressionBuilderParams(p, nonInlinePartWriterPinned);

                                var leftResult = CreateExpressionRecursive(np, arithmeticEvaluationStatement.Left);
                                var rightResult = CreateExpressionRecursive(np, arithmeticEvaluationStatement.Right);

                                if (leftResult.DataType.IsString() || rightResult.DataType.IsString())
                                {
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

                                    p.NonInlinePartWriter.Write(nonInlinePartWriterPinned);

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