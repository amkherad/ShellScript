using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashDefaultExpressionBuilder : ExpressionBuilderBase
    {
        public static BashDefaultExpressionBuilder Instance { get; } = new BashDefaultExpressionBuilder();

        public override string FormatExpression(Context context, Scope scope, string expression, IStatement template)
        {
            if (template is ConstantValueStatement)
                return expression;
            if (template is VariableAccessStatement)
                return expression;
            if (template is FunctionCallStatement)
                return expression;

            if (template is EvaluationStatement evaluationStatement)
            {
                if (evaluationStatement.GetDataType(context, scope).IsNumericOrFloat())
                {
                    if (expression.Contains("\""))
                    {
                        expression = expression.Replace('"', '\'');
                    }
                    
                    return $"`awk \"BEGIN {{print {expression}}}\"`";
                }
            }
            
            return base.FormatExpression(context, scope, expression, template);
        }
        
        public override string FormatExpression(Context context, Scope scope, DataTypes dataType, string expression, IStatement template)
        {
            if (template is ConstantValueStatement)
                return expression;
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
            
            return base.FormatExpression(context, scope, expression, template);
        }

        public override string PinExpressionToVariable(
            Context context,
            Scope scope,
            TextWriter writer,
            DataTypes dataTypes,
            string nameHint,
            string expression,
            IStatement template)
        {
            var variableName = scope.NewHelperVariable(dataTypes, nameHint);

            expression = $"${{{expression}}}";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                context,
                scope,
                writer,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(context, scope,
                variableName,
                template
            );
        }

        public override string PinFloatingPointExpressionToVariable(
            Context context,
            Scope scope,
            TextWriter writer,
            DataTypes dataTypes,
            string nameHint,
            string expression,
            IStatement template)
        {
            var variableName = scope.NewHelperVariable(dataTypes, nameHint);

            expression = $"`awk \"BEGIN {{print {expression}}}\"`";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                context,
                scope,
                writer,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(context, scope,
                variableName,
                template
            );
        }
    }
}