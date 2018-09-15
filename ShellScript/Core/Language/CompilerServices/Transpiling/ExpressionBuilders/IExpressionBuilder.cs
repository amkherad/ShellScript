using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public interface IExpressionBuilder
    {
        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes dataType, IStatement template);

        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes left, IStatement leftTemplate, DataTypes right, IStatement rightTemplate);


        string PinExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, IStatement template);

        string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            DataTypes dataTypes, string nameHint, string expression, IStatement template);

        string FormatExpression(ExpressionBuilderParams p, string expression, IStatement template);

        string FormatExpression(ExpressionBuilderParams p, DataTypes expressionDataType,
            string expression, IStatement template);

        string FormatSubExpression(ExpressionBuilderParams p, string expression, IStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, string expression, IStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left, IOperator op,
            DataTypes rightDataType, string right, IStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, string expression, IStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left, IOperator op,
            DataTypes rightDataType, string right, IStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, string expression, IStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, DataTypes leftDataType, string left, IOperator op,
            DataTypes rightDataType, string right, IStatement template);

        string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression, IStatement template);
        string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression, IStatement template);
        string FormatConstantExpression(ExpressionBuilderParams p, string expression, IStatement template);

        (DataTypes, string) CreateExpression(ExpressionBuilderParams p, IStatement statement);
    }
}