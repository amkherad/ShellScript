using System.IO;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashStringConcatenationExpressionBuilder : BashDefaultExpressionBuilder
    {
        public new static BashStringConcatenationExpressionBuilder Instance { get; } =
            new BashStringConcatenationExpressionBuilder();

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            var parentStatement = template.ParentStatement;
            if (parentStatement != null &&
                !(parentStatement is EvaluationStatement))
            {
                return base.FormatVariableAccessExpression(p, expression, template);
            }
            
            return $"${{{expression}}}";
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

                    return (DataTypes.String, constantValueStatement.Value, constantValueStatement);
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
                                    leftExp = FormatSubExpression(p,
                                        leftExp,
                                        left
                                    );
                                    rightExp = FormatSubExpression(p,
                                        rightExp,
                                        right
                                    );

                                    var exp = $"{leftExp}{rightExp}";

                                    p.NonInlinePartWriter.Write(nonInlinePartWriterPinned);

                                    return (
                                        DataTypes.String,
                                        exp,
                                        new ArithmeticEvaluationStatement(
                                            leftTemplate,
                                            arithmeticEvaluationStatement.Operator,
                                            rightTemplate,
                                            arithmeticEvaluationStatement.Info
                                        )
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