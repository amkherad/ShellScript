using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders
{
    public interface IExpressionBuilder
    {
        bool ShouldBePinnedToFloatingPointVariable(Context context, Scope scope,
            DataTypes dataType, IStatement template);

        bool ShouldBePinnedToFloatingPointVariable(Context context, Scope scope,
            DataTypes left, IStatement leftTemplate, DataTypes right, IStatement rightTemplate);


        string PinExpressionToVariable(Context context, Scope scope, TextWriter writer,
            DataTypes dataTypes, string nameHint, string expression, IStatement template);

        string PinFloatingPointExpressionToVariable(Context context, Scope scope,
            TextWriter writer, DataTypes dataTypes, string nameHint, string expression, IStatement template);

        string FormatExpression(Context context, Scope scope, string expression, IStatement template);

        string FormatExpression(Context context, Scope scope, DataTypes expressionDataType, string expression,
            IStatement template);

        string FormatSubExpression(Context context, Scope scope, string expression, IStatement template);

        string FormatArithmeticExpression(Context context, Scope scope, string expression, IStatement template);

        string FormatArithmeticExpression(Context context, Scope scope, DataTypes leftDataType, string left, IOperator op, DataTypes rightDataType,
            string right, IStatement template);

        string FormatBitwiseExpression(Context context, Scope scope, string expression, IStatement template);

        string FormatBitwiseExpression(Context context, Scope scope, DataTypes leftDataType, string left, IOperator op, DataTypes rightDataType,
            string right, IStatement template);

        string FormatLogicalExpression(Context context, Scope scope, string expression, IStatement template);

        string FormatLogicalExpression(Context context, Scope scope, DataTypes leftDataType, string left, IOperator op, DataTypes rightDataType,
            string right, IStatement template);

        string FormatVariableAccessExpression(Context context, Scope scope, string expression, IStatement template);
        string FormatFunctionCallExpression(Context context, Scope scope, string expression, IStatement template);
        string FormatConstantExpression(Context context, Scope scope, string expression, IStatement template);

        (DataTypes, string) CreateExpression(Context context, Scope scope,
            TextWriter nonInlinePartWriter, IStatement statement);
    }
}