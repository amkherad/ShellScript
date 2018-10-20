using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
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
            ExpressionResult left, ExpressionResult right)
        {
            if (left.DataType.IsNumericOrFloat() || right.DataType.IsNumericOrFloat())
            {
                if (template is LogicalEvaluationStatement)
                    return true;

                var parent = template.ParentStatement;

                if (parent is VariableDefinitionStatement)
                    return false;

                if (parent is ArithmeticEvaluationStatement arithmeticEvaluationStatement &&
                    arithmeticEvaluationStatement.Operator is AdditionOperator)
                    return arithmeticEvaluationStatement.Left.GetDataType(p.Context, p.Scope).IsString() ||
                           arithmeticEvaluationStatement.Right.GetDataType(p.Context, p.Scope).IsString();

                if (parent is FunctionCallStatement)
                    return true;

                return !(parent is EvaluationStatement);
            }

            return false;
        }

        public override bool ShouldBePinnedToFloatingPointVariable(
            ExpressionBuilderParams p, EvaluationStatement template,
            DataTypes left, EvaluationStatement leftTemplate, DataTypes right, EvaluationStatement rightTemplate)
        {
            if (left.IsNumericOrFloat() || right.IsNumericOrFloat())
            {
                if (template is LogicalEvaluationStatement)
                    return true;

                var parent = template.ParentStatement;

                if (parent is VariableDefinitionStatement)
                    return false;

                if (parent is ArithmeticEvaluationStatement arithmeticEvaluationStatement &&
                    arithmeticEvaluationStatement.Operator is AdditionOperator)
                    return arithmeticEvaluationStatement.Left.GetDataType(p.Context, p.Scope).IsString() ||
                           arithmeticEvaluationStatement.Right.GetDataType(p.Context, p.Scope).IsString();

                if (parent is FunctionCallStatement)
                    return true;

                return !(parent is EvaluationStatement);
            }

            return false;
        }


        public override string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.DataType.IsBoolean() &&
                result.Template is ConstantValueStatement constantValueStatement)
            {
                if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value, out var boolResult))
                {
                    throw new InvalidStatementStructureCompilerException(result.Template, result.Template.Info);
                }

                return boolResult ? "1" : "0";
            }
            
            return base.FormatConstantExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, DataTypes dataType, string expression,
            EvaluationStatement template)
        {
            if (dataType.IsBoolean() &&
                template is ConstantValueStatement constantValueStatement)
            {
                if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value, out var boolResult))
                {
                    throw new InvalidStatementStructureCompilerException(template, template.Info);
                }

                return boolResult ? "1" : "0";
            }

            
            return base.FormatConstantExpression(p, dataType, expression, template);
        }

        public override string FormatFunctionCallExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            return expression; //$"{expression}";
        }

        public override string FormatFunctionCallParameterSubExpression(ExpressionBuilderParams p,
            ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement)
                return result.Expression;
            if (result.Template is VariableAccessStatement)
                return result.Expression;
            if (result.Template is FunctionCallStatement)
                return result.Expression;

            if (result.DataType.IsNumericOrFloat())
            {
                return result.Expression;
            }

            return $"$(({result.Expression}))";
        }

        public override string FormatExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement || result.Template is VariableAccessStatement)
            {
                if (result.DataType.IsString())
                {
                    if (p.FormatString)
                    {
                        var value = result.Expression;
                        if (value[0] == '"' && value[value.Length - 1] == '"')
                        {
                            return value;
                        }
                        
                        return $"\"{value}\"";
                    }

                    return result.Expression;
                }

                return result.Expression;
            }

            //if (result.Template is VariableAccessStatement)
            //    return result.Expression;
            if (result.Template is FunctionCallStatement)
                return result.Expression;

            if (result.DataType.IsNumericOrFloat())
            {
                string expression;
                if (result.Expression.Contains("\""))
                {
                    expression = result.Expression.Replace('"', '\'');
                }
                else
                {
                    expression = result.Expression;
                }

                return $"`awk \"BEGIN {{print ({expression})}}\"`";
            }
            
            if (result.DataType.IsString())
            {
                if (p.FormatString)
                {
                    var value = result.Expression;
                    if (value[0] == '"' && value[value.Length - 1] == '"')
                    {
                        return value;
                    }
                    
                    return $"\"{value}\"";
                }

                return result.Expression;
            }
            
            return FormatEvaluationExpression(p, result);
        }

        protected virtual string FormatEvaluationExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            return $"$(({result.Expression}))";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            return '$' + expression;
        }


        public override string PinExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result)
        {
            var variableName = p.Scope.NewHelperVariable(result.DataType, nameHint);

            var expression = $"${{{result.Expression}}}";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(p,
                result.DataType,
                variableName,
                result.Template
            );
        }

        public override string PinExpressionToVariable(
            ExpressionBuilderParams p,
            DataTypes dataType,
            string nameHint,
            string expression,
            EvaluationStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(dataType, nameHint);

            expression = $"${{{expression}}}";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(p,
                dataType,
                variableName,
                template
            );
        }

        public override string PinFloatingPointExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result)
        {
            var variableName = p.Scope.NewHelperVariable(result.DataType, nameHint);

            var expression = $"`awk \"BEGIN {{print ({result.Expression})}}\"`";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(p,
                result.DataType,
                variableName,
                result.Template
            );
        }

        public override string PinFloatingPointExpressionToVariable(
            ExpressionBuilderParams p,
            DataTypes dataType,
            string nameHint,
            string expression,
            EvaluationStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(dataType, nameHint);

            expression = $"`awk \"BEGIN {{print ({expression})}}\"`";

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            return FormatVariableAccessExpression(p,
                dataType,
                variableName,
                template
            );
        }


        protected static string FormatStringConcatenationVariableAccess(string exp)
        {
            if (exp.StartsWith('$'))
            {
                if (exp.Length > 2 && exp[1] == '{')
                {
                    return exp;
                }
                
                return $"${{{exp.Substring(1)}}}";
            }

            return $"${{{exp}}}";
        }
    }
}