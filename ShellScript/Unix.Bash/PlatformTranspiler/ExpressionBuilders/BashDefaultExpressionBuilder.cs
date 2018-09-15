using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashDefaultExpressionBuilder : ExpressionBuilderBase
    {
        public static BashDefaultExpressionBuilder Instance { get; } = new BashDefaultExpressionBuilder();

        public override string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression,
            IStatement template)
        {
            return $"$(({expression}))";
        }

        public override string FormatExpression(ExpressionBuilderParams p, string expression, IStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.GetDataType(p.Context, p.Scope).IsString())
                    return $"\"{expression}\"";
                
                return expression;
            }

            if (template is VariableAccessStatement)
                return expression;
            if (template is FunctionCallStatement)
                return expression;

            if (template is EvaluationStatement evaluationStatement)
            {
                if (evaluationStatement.GetDataType(p.Context, p.Scope).IsNumericOrFloat())
                {
                    if (expression.Contains("\""))
                    {
                        expression = expression.Replace('"', '\'');
                    }

                    return $"`awk \"BEGIN {{print {expression}}}\"`";
                }
            }

            return base.FormatExpression(p, expression, template);
        }

        public override string FormatExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            IStatement template)
        {
            if (template is ConstantValueStatement)
            {
                if (dataType.IsString())
                    return $"\"{expression}\"";
                
                return expression;
            }

            if (template is VariableAccessStatement)
                return expression;
            if (template is FunctionCallStatement)
                return expression;

            if (dataType.IsNumericOrFloat())
            {
                if (expression.Contains("\""))
                {
                    expression = expression.Replace('"', '\'');
                }

                return $"`awk \"BEGIN {{print {expression}}}\"`";
            }

            return base.FormatExpression(p, expression, template);
        }

        public override string PinExpressionToVariable(
            ExpressionBuilderParams p,
            DataTypes dataTypes,
            string nameHint,
            string expression,
            IStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(dataTypes, nameHint);

            expression = $"${{{expression}}}";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(p,
                variableName,
                template
            );
        }

        public override string PinFloatingPointExpressionToVariable(
            ExpressionBuilderParams p,
            DataTypes dataTypes,
            string nameHint,
            string expression,
            IStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(dataTypes, nameHint);

            expression = $"`awk \"BEGIN {{print {expression}}}\"`";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(p,
                variableName,
                template
            );
        }
    }
}