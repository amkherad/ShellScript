using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashConditionalStringConcatenationExpressionBuilder : BashConditionalExpressionBuilder
    {
        public new static BashConditionalStringConcatenationExpressionBuilder Instance { get; } =
            new BashConditionalStringConcatenationExpressionBuilder();

        public override string FormatSubExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template)
        {
            if (dataType == DataTypes.String)
            {
                return expression;
            }
            
            return base.FormatSubExpression(p, dataType, expression, template);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                }
            }

            return base.FormatConstantExpression(p, expression, template);
        }

        protected override (DataTypes, string, EvaluationStatement) CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    if (constantValueStatement.IsString())
                    {
                        return (
                            DataTypes.String,
                            BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false),
                            constantValueStatement
                        );
                    }

                    return (constantValueStatement.DataType, constantValueStatement.Value, constantValueStatement);
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case AdditionOperator _:
                        {
                            using (var nonInlinePartWriterPinned = new StringWriter())
                            {
                                var left = arithmeticEvaluationStatement.Left;
                                var right = arithmeticEvaluationStatement.Right;

                                var np = new ExpressionBuilderParams(p, nonInlinePartWriterPinned);
                                
                                var (leftDataType, leftExp, leftTemplate) = CreateExpressionRecursive(np, left);
                                var (rightDataType, rightExp, rightTemplate) = CreateExpressionRecursive(np, right);

                                if (leftDataType.IsString() || rightDataType.IsString())
                                {
                                    if (leftTemplate is VariableAccessStatement)
                                    {
                                        leftExp = FormatStringConcatenationVariableAccess(leftExp);
                                    }
                                    if (rightTemplate is VariableAccessStatement)
                                    {
                                        rightExp = FormatStringConcatenationVariableAccess(rightExp);
                                    }
                                    

                                    if (leftDataType != DataTypes.String && !(leftTemplate is VariableAccessStatement))
                                    {
                                        leftExp = $"$(({leftExp}))";
                                    }
                                    if (rightDataType != DataTypes.String && !(rightTemplate is VariableAccessStatement))
                                    {
                                        rightExp = $"$(({rightExp}))";
                                    }

                                    var concatExp = $"{leftExp}{rightExp}";

                                    p.NonInlinePartWriter.Write(nonInlinePartWriterPinned);

                                    var newTemp = new ArithmeticEvaluationStatement(
                                        leftTemplate,
                                        arithmeticEvaluationStatement.Operator,
                                        rightTemplate,
                                        arithmeticEvaluationStatement.Info
                                    );

                                    leftTemplate.ParentStatement = newTemp;
                                    rightTemplate.ParentStatement = newTemp;

                                    return (
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