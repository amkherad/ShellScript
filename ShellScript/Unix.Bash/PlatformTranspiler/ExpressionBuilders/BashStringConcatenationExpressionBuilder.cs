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

        public override string FormatVariableAccessExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            return $"${{{expression}}}";
        }

        public override string FormatConstantExpression(Context context, Scope scope, string expression,
            IStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                {
                    return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                }
            }

            return base.FormatConstantExpression(context, scope, expression, template);
        }

        protected override (DataTypes, string) CreateExpressionRecursive(Context context, Scope scope,
            TextWriter nonInlinePartWriter,
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

                                var (leftDataType, leftExp) = CreateExpressionRecursive(context, scope,
                                    nonInlinePartWriterPinned, left);
                                var (rightDataType, rightExp) = CreateExpressionRecursive(context, scope,
                                    nonInlinePartWriterPinned, right);

                                if (leftDataType.IsString() || rightDataType.IsString())
                                {
                                    leftExp = FormatSubExpression(context, scope,
                                        leftExp,
                                        left
                                    );
                                    rightExp = FormatSubExpression(context, scope,
                                        rightExp,
                                        right
                                    );

                                    var exp = $"{leftExp}{rightExp}";

                                    nonInlinePartWriter.Write(nonInlinePartWriterPinned);

                                    return (DataTypes.String, exp);
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            return base.CreateExpressionRecursive(context, scope, nonInlinePartWriter, statement);
        }
    }
}