using System.IO;
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

        public override string FormatSubExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.DataType == DataTypes.String)
            {
                return result.Expression;
            }

            return base.FormatSubExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement constantValueStatement)
            {
                if (result.DataType.IsBoolean())
                {
                    if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value, out var boolResult))
                    {
                        throw new InvalidStatementStructureCompilerException(result.Template, result.Template.Info);
                    }

                    return boolResult ? "1" : "0";
                }

                if (constantValueStatement.IsString())
                {
                    if (result.Template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement)
                    {
                        if (arithmeticEvaluationStatement.Operator is AdditionOperator)
                        {
                            if (p.FormatString)
                                return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                        }
                        else
                        {
                            throw new InvalidStatementStructureCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }
                    }

                    return base.FormatConstantExpression(p, result);
                }
            }

            return base.FormatConstantExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression,
            EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (dataType.IsBoolean())
                {
                    if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value, out var boolResult))
                    {
                        throw new InvalidStatementStructureCompilerException(template, template.Info);
                    }

                    return boolResult ? "1" : "0";
                }

                if (constantValueStatement.IsString())
                {
                    if (template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement)
                    {
                        if (arithmeticEvaluationStatement.Operator is AdditionOperator)
                        {
                            if (p.FormatString)
                                return BashTranspilerHelpers.ToBashString(constantValueStatement.Value, true, false);
                        }
                        else
                        {
                            throw new InvalidStatementStructureCompilerException(arithmeticEvaluationStatement,
                                arithmeticEvaluationStatement.Info);
                        }
                    }

                    return base.FormatConstantExpression(p, dataType, expression, template);
                }
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

            if (result.DataType.IsString())
            {
                return $"\"{result.Expression}\"";
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
        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            var exp = '$' + result.Expression;

            if (!result.DataType.IsString())
            {
                return exp;
            }

            if (result.Template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement &&
                arithmeticEvaluationStatement.Operator is AdditionOperator)
            {
                return exp;
            }

//            if (result.Template.ParentStatement is AssignmentStatement ||
//                result.Template.ParentStatement is VariableDefinitionStatement)
//            {
//                return $"\"{base.FormatVariableAccessExpression(p, result)}\"";
//            }

            if (exp[0] == '"' && exp[exp.Length - 1] == '"')
            {
                return exp;
            }

            return $"\"{exp}\"";
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, DataTypes dataType,
            string expression, EvaluationStatement template)
        {
            var exp = '$' + expression;

            if (!dataType.IsString())
            {
                return exp;
            }

            if (template.ParentStatement is ArithmeticEvaluationStatement arithmeticEvaluationStatement &&
                arithmeticEvaluationStatement.Operator is AdditionOperator)
            {
                return exp;
            }

//            if (template.ParentStatement is AssignmentStatement ||
//                template.ParentStatement is VariableDefinitionStatement)
//            {
//                return $"\"{base.FormatVariableAccessExpression(p, dataType, expression, template)}\"";
//            }

            if (exp[0] == '"' && exp[exp.Length - 1] == '"')
            {
                return exp;
            }

            return $"\"{exp}\"";
        }


        public override PinnedVariableResult PinExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result)
        {
            var variableName = p.Scope.NewHelperVariable(result.DataType, nameHint);

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                result.Expression
            );

            var template = new VariableAccessStatement(variableName, result.Template.Info);

            return new PinnedVariableResult(
                result.DataType,
                variableName,
                FormatVariableAccessExpression(p,
                    result.DataType,
                    variableName,
                    template
                ),
                template);
        }

        public override PinnedVariableResult PinExpressionToVariable(
            ExpressionBuilderParams p,
            DataTypes dataType,
            string nameHint,
            string expression,
            EvaluationStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(dataType, nameHint);

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            template = new VariableAccessStatement(variableName, template.Info);

            return new PinnedVariableResult(
                dataType,
                variableName,
                FormatVariableAccessExpression(p,
                    dataType,
                    variableName,
                    template
                ),
                template);
        }

        public override PinnedVariableResult PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            string nameHint, ExpressionResult result)
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

            var template = new VariableAccessStatement(variableName, result.Template.Info);

            return new PinnedVariableResult(
                result.DataType,
                variableName,
                FormatVariableAccessExpression(p,
                    result.DataType,
                    variableName,
                    template
                ),
                template);
        }

        public override PinnedVariableResult PinFloatingPointExpressionToVariable(
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

            template = new VariableAccessStatement(variableName, template.Info);

            return new PinnedVariableResult(
                dataType,
                variableName,
                FormatVariableAccessExpression(p,
                    dataType,
                    variableName,
                    template
                ),
                template);
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


        protected override ExpressionResult CreateExpressionRecursive(ExpressionBuilderParams px,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (px.UsageContext is IfElseStatement)
                    {
                        break;
                    }

                    switch (logicalEvaluationStatement.Operator)
                    {
                        case EqualOperator _:
                        case NotEqualOperator _:
                        {
                            using (var writer = new StringWriter())
                            {
                                var p = new ExpressionBuilderParams(px, writer);

                                var leftResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Left);
                                var rightResult = CreateExpressionRecursive(p, logicalEvaluationStatement.Right);

                                if (leftResult.DataType.IsString() && rightResult.DataType.IsString())
                                {
                                    HandleNotices(p, ref leftResult, ref rightResult);

                                    p.NonInlinePartWriter.WriteLine(
                                        $"[ {leftResult.Expression} {logicalEvaluationStatement.Operator} {rightResult.Expression} ]");

                                    string varName;

                                    if (logicalEvaluationStatement.ParentStatement is AssignmentStatement ||
                                        logicalEvaluationStatement.ParentStatement is VariableDefinitionStatement)
                                    {
                                        varName = UnixBashPlatform.LastStatusCodeStoreVariableName;
                                    }
                                    else
                                    {
                                        varName = BashVariableDefinitionStatementTranspiler
                                            .WriteLastStatusCodeStoreVariableDefinition(p.Context, p.Scope,
                                                p.NonInlinePartWriter, "cmp_strings");
                                    }

                                    var template = new VariableAccessStatement(
                                        varName,
                                        logicalEvaluationStatement.Info
                                    );

                                    px.NonInlinePartWriter.Write(writer);
                                    
                                    return new ExpressionResult(
                                        DataTypes.Boolean,
                                        $"${varName}",
                                        template,
                                        PinRequiredNotice
                                    );
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
                case ArithmeticEvaluationStatement arithmeticEvaluationStatement:
                {
                    switch (arithmeticEvaluationStatement.Operator)
                    {
                        case AdditionOperator _:
                        {
                            using (var writer = new StringWriter())
                            {
                                var p = new ExpressionBuilderParams(px, writer);

                                var leftResult = CreateExpressionRecursive(p, arithmeticEvaluationStatement.Left);
                                var rightResult = CreateExpressionRecursive(p, arithmeticEvaluationStatement.Right);

                                if (leftResult.DataType.IsString() || rightResult.DataType.IsString())
                                {
                                    HandleNotices(p, ref leftResult, ref rightResult);

                                    var leftExp = leftResult.Expression;
                                    var rightExp = rightResult.Expression;

                                    if (leftResult.Template is VariableAccessStatement)
                                    {
                                        leftExp = FormatStringConcatenationVariableAccess(leftExp);
                                    }

                                    if (rightResult.Template is VariableAccessStatement)
                                    {
                                        rightExp = FormatStringConcatenationVariableAccess(rightExp);
                                    }


                                    if (leftResult.DataType != DataTypes.String &&
                                        !(leftResult.Template is VariableAccessStatement))
                                    {
                                        leftExp = $"$(({leftExp}))";
                                    }

                                    if (rightResult.DataType != DataTypes.String &&
                                        !(rightResult.Template is VariableAccessStatement))
                                    {
                                        rightExp = $"$(({rightExp}))";
                                    }

                                    var concatExp = $"{leftExp}{rightExp}";

                                    var newTemp = new ArithmeticEvaluationStatement(
                                        leftResult.Template,
                                        arithmeticEvaluationStatement.Operator,
                                        rightResult.Template,
                                        arithmeticEvaluationStatement.Info
                                    );

                                    leftResult.Template.ParentStatement = newTemp;
                                    rightResult.Template.ParentStatement = newTemp;

                                    px.NonInlinePartWriter.Write(writer);

                                    return new ExpressionResult(
                                        DataTypes.String,
                                        concatExp,
                                        newTemp
                                    );
                                }
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            return base.CreateExpressionRecursive(px, statement);
        }
    }
}