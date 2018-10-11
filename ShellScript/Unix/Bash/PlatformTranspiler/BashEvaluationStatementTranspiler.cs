using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Statements.Operators;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.Unix.Bash.PlatformTranspiler
{
    public class BashEvaluationStatementTranspiler : EvaluationStatementTranspilerBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionResult CreateBashExpression(ExpressionBuilderParams p,
            EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(p.Context, p.Scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(p.Context, p.Scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashStringConcatenationExpressionBuilder.Instance
                : BashDefaultExpressionBuilder.Instance;

            return expressionBuilder.CreateExpression(p, evalStt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionResult CreateBashConditionalExpression(
            ExpressionBuilderParams p, EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(p.Context, p.Scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(p.Context, p.Scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashConditionalStringConcatenationExpressionBuilder.Instance
                : BashConditionalExpressionBuilder.Instance;

            return expressionBuilder.CreateExpression(p, evalStt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IExpressionBuilder GetBashExpressionBuilder(Context context, Scope scope,
            ref EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(context, scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(context, scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashStringConcatenationExpressionBuilder.Instance
                : BashDefaultExpressionBuilder.Instance;

            return expressionBuilder;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IExpressionBuilder GetBashConditionalExpressionBuilder(Context context, Scope scope,
            ref EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(context, scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(context, scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashConditionalStringConcatenationExpressionBuilder.Instance
                : BashConditionalExpressionBuilder.Instance;

            return expressionBuilder;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            var parameters = new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, null);

            var expression = CreateBashExpression(parameters, evalStt);

            writer.Write(expression);
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override string PinEvaluationToVariable(Context context, Scope scope, TextWriter metaWriter,
            TextWriter pinCodeWriter, EvaluationStatement statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            var expressionBuilder = GetBashExpressionBuilder(context, scope, ref statement);

            var parameters = new ExpressionBuilderParams(context, scope, metaWriter, pinCodeWriter, null);

            var result = expressionBuilder.CreateExpression(parameters, statement);

            return expressionBuilder.PinExpressionToVariable(parameters, null, result);
        }

        public override ExpressionResult GetExpression(ExpressionBuilderParams p,
            EvaluationStatement statement) =>
            CreateBashExpression(p, statement);

        public override ExpressionResult GetExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext,
            EvaluationStatement statement) =>
            CreateBashExpression(
                new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, usageContext), statement);

        public override ExpressionResult GetConditionalExpression(ExpressionBuilderParams p,
            EvaluationStatement statement) =>
            CreateBashConditionalExpression(p, statement);

        public override ExpressionResult GetConditionalExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext,
            EvaluationStatement statement) =>
            CreateBashConditionalExpression(
                new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, usageContext), statement);
    }
}