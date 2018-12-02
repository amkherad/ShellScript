using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders
{
    public interface IExpressionBuilder
    {
        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
            EvaluationStatement template);

        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, EvaluationStatement template,
            ExpressionResult left, ExpressionResult right);

        bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p, EvaluationStatement template,
            TypeDescriptor left, EvaluationStatement leftTemplate, TypeDescriptor right,
            EvaluationStatement rightTemplate);


        PinnedVariableResult PinExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result);

        PinnedVariableResult PinExpressionToVariable(ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor, string nameHint, string expression, EvaluationStatement template);

        PinnedVariableResult PinFloatingPointExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result);

        PinnedVariableResult PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor, string nameHint, string expression, EvaluationStatement template);


        string FormatExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatSubExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p, ExpressionResult result);


        string FormatArithmeticExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, TypeDescriptor leftTypeDescriptor, string leftExp,
            IOperator op, TypeDescriptor rightTypeDescriptor, string rightExp, EvaluationStatement template);

        string FormatArithmeticExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template);


        string FormatBitwiseExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, TypeDescriptor leftTypeDescriptor, string leftExp,
            IOperator op, TypeDescriptor rightTypeDescriptor, string rightExp, EvaluationStatement template);

        string FormatBitwiseExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template);


        string FormatLogicalExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, TypeDescriptor leftTypeDescriptor, string leftExp,
            IOperator op, TypeDescriptor rightTypeDescriptor, string rightExp, EvaluationStatement template);

        string FormatLogicalExpression(ExpressionBuilderParams p, ExpressionResult left, IOperator op,
            ExpressionResult right, EvaluationStatement template);


        string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatVariableAccessExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
            string expression, EvaluationStatement template);

        string FormatArrayAccessExpression(ExpressionBuilderParams p, ExpressionResult source,
            ExpressionResult indexer);

        string FormatFunctionCallExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatFunctionCallExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor, string expression,
            EvaluationStatement template);


        string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result);

        string FormatConstantExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor, string expression,
            EvaluationStatement template);

        ExpressionResult CallApiFunction<TApiFunc>(ExpressionBuilderParams p,
            EvaluationStatement[] parameters, IStatement parentStatement, StatementInfo statementInfo)
            where TApiFunc : IApiFunc;

        ExpressionResult CreateExpression(ExpressionBuilderParams p, EvaluationStatement statement);
    }
}