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

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return $"${{{expression}}}";
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
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

        protected override (DataTypes, string) CreateExpressionRecursive(ExpressionBuilderParams p,
            IStatement statement)
        {
            switch (statement)
            {
                case ConstantValueStatement constantValueStatement:
                {
                    if (constantValueStatement.IsString())
                    {
                        return (DataTypes.String,
                            BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false));
                    }

                    return (DataTypes.String, constantValueStatement.Value);
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
                                
                                var (leftDataType, leftExp) = CreateExpressionRecursive(np, left);
                                var (rightDataType, rightExp) = CreateExpressionRecursive(np, right);

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

                                    return (DataTypes.String, exp);
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