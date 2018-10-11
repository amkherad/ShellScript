using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public interface IExpressionBuilder
    {
        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, DataTypes dataType,
            EvaluationStatement template);

        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, EvaluationStatement template,
            ExpressionResult left, ExpressionResult right);

        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, EvaluationStatement template,
            DataTypes left, EvaluationStatement leftTemplate, DataTypes right, EvaluationStatement rightTemplate);



        string PinExpressionToVariable(ExpressionBuilderParams p, string nameHint, ExpressionResult result);

        string PinExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, EvaluationStatement template);

        string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result);
        
        string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, EvaluationStatement template);


        string FormatExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatSubExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p, ExpressionResult result);


        string FormatArithmeticExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, DataTypes leftDataType, string leftExp,
            IOperator op, DataTypes rightDataType, string rightExp, EvaluationStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template);


        string FormatBitwiseExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, DataTypes leftDataType, string leftExp,
            IOperator op, DataTypes rightDataType, string rightExp, EvaluationStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template);


        string FormatLogicalExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, DataTypes leftDataType, string leftExp,
            IOperator op, DataTypes rightDataType, string rightExp, EvaluationStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template);


        string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template);


        string FormatFunctionCallExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);


        string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatConstantExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);


        ExpressionResult CreateExpression(ExpressionBuilderParams p, EvaluationStatement statement);
    }
}