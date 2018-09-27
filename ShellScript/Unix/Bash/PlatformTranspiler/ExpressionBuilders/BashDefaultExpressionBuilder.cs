using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashDefaultExpressionBuilder : ExpressionBuilderBase
    {
        public static BashDefaultExpressionBuilder Instance { get; } = new BashDefaultExpressionBuilder();

        
        public override bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            DataTypes dataType, EvaluationStatement template)
        {
            if (template is ConstantValueStatement)
                return false;
            if (template is VariableAccessStatement)
                return false;
            if (template is FunctionCallStatement)
                return false;

            return dataType.IsNumericOrFloat();
        }

        public override bool ShouldBePinnedToFloatingPointVariable(
            ExpressionBuilderParams p, EvaluationStatement template,
            DataTypes left, EvaluationStatement leftTemplate, DataTypes right, EvaluationStatement rightTemplate)
        {
            var test = right.IsNumericOrFloat() || left.IsNumericOrFloat();
            if (test)
            {
                return
                    template.ParentStatement is FunctionCallStatement;
            }

            return false;
        }
        
        
        public override string FormatFunctionCallExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return expression; //$"{expression}";
        }
        
        public override string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            if (template is ConstantValueStatement)
                return expression;
            if (template is VariableAccessStatement)
                return expression;
            if (template is FunctionCallStatement)
                return expression;

            if (dataType.IsNumericOrFloat())
            {
                return expression;
            }
            
            return $"$(({expression}))";
        }

        public override string FormatExpression(ExpressionBuilderParams p, string expression, EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (constantValueStatement.IsString())
                    return $"\"{expression}\"";
                
                return expression;
            }

            if (template is VariableAccessStatement)
                return expression;
            if (template is FunctionCallStatement)
                return expression;

            if (template.GetDataType(p.Context, p.Scope).IsNumericOrFloat())
            {
                if (expression.Contains("\""))
                {
                    expression = expression.Replace('"', '\'');
                }

                return $"`awk \"BEGIN {{print {expression}}}\"`";
            }

            return base.FormatExpression(p, expression, template);
        }

        public override string FormatExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template)
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

            return base.FormatExpression(p, dataType, expression, template);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, string expression,
            EvaluationStatement template)
        {
            return '$' + expression;
        }
        

        public override string PinExpressionToVariable(
            ExpressionBuilderParams p,
            DataTypes dataTypes,
            string nameHint,
            string expression,
            EvaluationStatement template)
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
            EvaluationStatement template)
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