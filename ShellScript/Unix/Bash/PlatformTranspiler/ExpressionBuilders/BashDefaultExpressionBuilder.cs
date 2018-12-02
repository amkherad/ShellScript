using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ShellScript.Core;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Array;

namespace ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders
{
    public class BashDefaultExpressionBuilder : ExpressionBuilderBase
    {
        public static BashDefaultExpressionBuilder Instance { get; } = new BashDefaultExpressionBuilder();


        public override bool ShouldBePinnedToFloatingPointVariable(ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor, EvaluationStatement template)
        {
            if (template is ConstantValueStatement)
                return false;
            if (template is VariableAccessStatement)
                return false;
            if (template is FunctionCallStatement)
                return false;

            return typeDescriptor.IsNumericOrFloat();
        }

        public override bool ShouldBePinnedToFloatingPointVariable(
            ExpressionBuilderParams p, EvaluationStatement template,
            ExpressionResult left, ExpressionResult right)
        {
            if (left.TypeDescriptor.IsNumericOrFloat() || right.TypeDescriptor.IsNumericOrFloat())
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
            TypeDescriptor left, EvaluationStatement leftTemplate, TypeDescriptor right,
            EvaluationStatement rightTemplate)
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
            if (result.TypeDescriptor.IsString())
            {
                return result.Expression;
            }

            return base.FormatSubExpression(p, result);
        }

        public override string FormatConstantExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement constantValueStatement)
            {
                if (result.TypeDescriptor.IsBoolean())
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

        public override string FormatConstantExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
            string expression,
            EvaluationStatement template)
        {
            if (template is ConstantValueStatement constantValueStatement)
            {
                if (typeDescriptor.IsBoolean())
                {
                    if (!StatementHelpers.TryParseBooleanFromString(constantValueStatement.Value, out var boolResult))
                    {
                        throw new InvalidStatementStructureCompilerException(template, template.Info);
                    }

                    return boolResult ? "1" : "0";
                }

                if (constantValueStatement.IsString() || constantValueStatement.IsDelegate())
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

                    return base.FormatConstantExpression(p, typeDescriptor, expression, template);
                }
            }

            return base.FormatConstantExpression(p, typeDescriptor, expression, template);
        }

        public override string FormatFunctionCallExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
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

            if (result.TypeDescriptor.IsNumericOrFloat())
            {
                return result.Expression;
            }

            if (result.TypeDescriptor.IsString())
            {
                return $"\"{result.Expression}\"";
            }

            return $"$(({result.Expression}))";
        }

        public override string FormatExpression(ExpressionBuilderParams p, ExpressionResult result)
        {
            if (result.Template is ConstantValueStatement || result.Template is VariableAccessStatement ||
                result.Template is IndexerAccessStatement || result.Template is ArrayStatement)
            {
                if (result.TypeDescriptor.IsString())
                {
                    if (p.FormatString)
                    {
                        return StringHelpers.EnQuote(result.Expression);
                    }

                    return result.Expression;
                }

                return result.Expression;
            }

            //if (result.Template is VariableAccessStatement)
            //    return result.Expression;
            if (result.Template is FunctionCallStatement)
                return result.Expression;

            if (result.TypeDescriptor.IsNumericOrFloat())
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

            if (result.TypeDescriptor.IsString())
            {
                if (p.FormatString)
                {
                    return StringHelpers.EnQuote(result.Expression);
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
            //$10 should be surrounded by braces or it will considered $1 followed by a zero
            var expression = result.Expression;
            var exp = expression.All(char.IsDigit) && expression.Length > 1
                ? $"${{{expression}}}"
                : '$' + expression;

            if (!result.TypeDescriptor.IsString())
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

            return StringHelpers.EnQuote(exp);
        }

        public override string FormatVariableAccessExpression(ExpressionBuilderParams p, TypeDescriptor typeDescriptor,
            string expression, EvaluationStatement template)
        {
            //$10 should be surrounded by braces or it will considered $1 followed by a zero
            var exp = expression.All(char.IsDigit) && expression.Length > 1
                ? $"${{{expression}}}"
                : '$' + expression;

            if (!typeDescriptor.IsString())
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

            return StringHelpers.EnQuote(exp);
        }

        public override string FormatArrayAccessExpression(ExpressionBuilderParams p, ExpressionResult source,
            ExpressionResult indexer)
        {
            var sExp = source.Expression;
            if (sExp.StartsWith('$'))
            {
                sExp = sExp.Substring(1);
            }

            return $"${{{sExp}[{indexer.Expression}]}}";
        }

        public override PinnedVariableResult PinExpressionToVariable(ExpressionBuilderParams p, string nameHint,
            ExpressionResult result)
        {
            var variableName = p.Scope.NewHelperVariable(result.TypeDescriptor, nameHint);

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                result.Expression
            );

            var template = new VariableAccessStatement(variableName, result.Template.Info);

            return new PinnedVariableResult(
                result.TypeDescriptor,
                variableName,
                FormatVariableAccessExpression(p,
                    result.TypeDescriptor,
                    variableName,
                    template
                ),
                template);
        }

        public override PinnedVariableResult PinExpressionToVariable(
            ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor,
            string nameHint,
            string expression,
            EvaluationStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(typeDescriptor, nameHint);

            BashVariableDefinitionStatementTranspiler.WriteVariableDefinition(
                p.Context,
                p.Scope,
                p.NonInlinePartWriter,
                variableName,
                expression
            );

            template = new VariableAccessStatement(variableName, template.Info);

            return new PinnedVariableResult(
                typeDescriptor,
                variableName,
                FormatVariableAccessExpression(p,
                    typeDescriptor,
                    variableName,
                    template
                ),
                template);
        }

        public override PinnedVariableResult PinFloatingPointExpressionToVariable(ExpressionBuilderParams p,
            string nameHint, ExpressionResult result)
        {
            var variableName = p.Scope.NewHelperVariable(result.TypeDescriptor, nameHint);

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
                result.TypeDescriptor,
                variableName,
                FormatVariableAccessExpression(p,
                    result.TypeDescriptor,
                    variableName,
                    template
                ),
                template);
        }

        public override PinnedVariableResult PinFloatingPointExpressionToVariable(
            ExpressionBuilderParams p,
            TypeDescriptor typeDescriptor,
            string nameHint,
            string expression,
            EvaluationStatement template)
        {
            var variableName = p.Scope.NewHelperVariable(typeDescriptor, nameHint);

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
                typeDescriptor,
                variableName,
                FormatVariableAccessExpression(p,
                    typeDescriptor,
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


        protected override ExpressionResult CreateExpressionRecursive(ExpressionBuilderParams p,
            EvaluationStatement statement)
        {
            switch (statement)
            {
                case LogicalEvaluationStatement logicalEvaluationStatement:
                {
                    if (p.UsageContext is IfElseStatement)
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
                                var expressionBuilderParams = new ExpressionBuilderParams(p, writer);

                                var leftResult = CreateExpressionRecursive(expressionBuilderParams,
                                    logicalEvaluationStatement.Left);
                                var rightResult = CreateExpressionRecursive(expressionBuilderParams,
                                    logicalEvaluationStatement.Right);

                                if (leftResult.TypeDescriptor.IsString() && rightResult.TypeDescriptor.IsString())
                                {
                                    HandleNotices(expressionBuilderParams, ref leftResult, ref rightResult);

                                    expressionBuilderParams.NonInlinePartWriter.WriteLine(
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
                                            .WriteLastStatusCodeStoreVariableDefinition(expressionBuilderParams.Context,
                                                expressionBuilderParams.Scope,
                                                expressionBuilderParams.NonInlinePartWriter, "cmp_strings");
                                    }

                                    var template = new VariableAccessStatement(
                                        varName,
                                        logicalEvaluationStatement.Info
                                    );

                                    p.NonInlinePartWriter.Write(writer);

                                    return new ExpressionResult(
                                        TypeDescriptor.Boolean,
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
                                var expressionBuilderParams = new ExpressionBuilderParams(p, writer);

                                var leftResult = CreateExpressionRecursive(expressionBuilderParams,
                                    arithmeticEvaluationStatement.Left);
                                var rightResult = CreateExpressionRecursive(expressionBuilderParams,
                                    arithmeticEvaluationStatement.Right);

                                if (leftResult.TypeDescriptor.IsString() || rightResult.TypeDescriptor.IsString())
                                {
                                    HandleNotices(expressionBuilderParams, ref leftResult, ref rightResult);

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


                                    if (!leftResult.TypeDescriptor.IsString() &&
                                        !(leftResult.Template is VariableAccessStatement))
                                    {
                                        leftExp = $"$(({leftExp}))";
                                    }

                                    if (!rightResult.TypeDescriptor.IsString() &&
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

                                    p.NonInlinePartWriter.Write(writer);

                                    return new ExpressionResult(
                                        TypeDescriptor.String,
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

                case ArrayStatement arrayStatement:
                {
                    string sourceName;

                    if (arrayStatement.ParentStatement is VariableDefinitionStatement variableDefinitionStatement)
                    {
                        sourceName = variableDefinitionStatement.Name;
                    }
                    else if (arrayStatement.ParentStatement is AssignmentStatement assignmentStatement)
                    {
                        if (!(assignmentStatement.LeftSide is VariableAccessStatement variableAccessStatement))
                        {
                            throw new InvalidStatementStructureCompilerException(assignmentStatement,
                                assignmentStatement.Info);
                        }

                        sourceName = variableAccessStatement.VariableName;
                    }
                    else
                    {
                        sourceName = p.Scope.NewHelperVariable(arrayStatement.Type, "array_helper");
                    }

                    if (arrayStatement.Elements != null)
                    {
                        for (var i = 0; i < arrayStatement.Elements.Length; i++)
                        {
                            p.NonInlinePartWriter.Write($"{sourceName}[{i}]=");

                            var element = CreateExpression(p, arrayStatement.Elements[i]);

                            p.NonInlinePartWriter.WriteLine(element);
                        }

                        return ExpressionResult.EmptyResult;
                    }

                    if (arrayStatement.Length != null)
                    {
                        var sourceNameEval = new VariableAccessStatement(sourceName, arrayStatement.Info);

                        return CallApiFunction<ApiArray.Initialize>(p, new[] {sourceNameEval, arrayStatement.Length},
                            arrayStatement.ParentStatement, arrayStatement.Info);
                    }

                    throw new InvalidStatementStructureCompilerException(arrayStatement, arrayStatement.Info);
                }
            }

            return base.CreateExpressionRecursive(p, statement);
        }
    }
}