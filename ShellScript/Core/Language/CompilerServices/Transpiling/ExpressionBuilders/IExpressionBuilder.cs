using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public interface IExpressionBuilder
    {
        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes dataType, EvaluationStatement template);

        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, EvaluationStatement template,
            DataTypes left, EvaluationStatement leftTemplate, DataTypes right, EvaluationStatement rightTemplate);


        string PinExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, EvaluationStatement template);

        string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, EvaluationStatement template);

        string FormatExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatExpression(ExpressionBuilderParams p, DataTypes expressionDataType,
            string expression, EvaluationStatement template);

        string FormatSubExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template);

        string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left, IOperator op,
            DataTypes rightDataType, string right, EvaluationStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left, IOperator op,
            DataTypes rightDataType, string right, EvaluationStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left, IOperator op,
            DataTypes rightDataType, string right, EvaluationStatement template);

        string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template);

        string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);
        string FormatConstantExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        (DataTypes, string, EvaluationStatement) CreateExpression(ExpressionBuilderParams p,
            EvaluationStatement statement);
    }
}