using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Statements.Operators;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Windows.Batch.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.Windows.Batch.PlatformTranspiler
{
    public class BatchEvaluationStatementTranspiler : EvaluationStatementTranspilerBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionResult CreateBatchExpression(ExpressionBuilderParams p,
            EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(p.Context, p.Scope, evalStt);

            return BatchDefaultExpressionBuilder.Instance.CreateExpression(p, evalStt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionResult CreateBatchConditionalExpression(ExpressionBuilderParams p,
            EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(p.Context, p.Scope, evalStt);

            return BatchConditionalExpressionBuilder.Instance.CreateExpression(p, evalStt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IExpressionBuilder GetBatchExpressionBuilder(Context context, Scope scope,
            ref EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(context, scope, evalStt);

            return BatchDefaultExpressionBuilder.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IExpressionBuilder GetBatchConditionalExpressionBuilder(Context context, Scope scope,
            ref EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(context, scope, evalStt);

            return BatchConditionalExpressionBuilder.Instance;
        }

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            if (!(statement is EvaluationStatement evalStt)) throw new InvalidOperationException();

            var parameters = new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, null);

            var expression = CreateBatchExpression(parameters, evalStt);

            writer.Write(expression);
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override PinnedVariableResult PinEvaluationToVariable(Context context, Scope scope,
            TextWriter metaWriter,
            TextWriter pinCodeWriter, EvaluationStatement statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            var expressionBuilder = GetBatchExpressionBuilder(context, scope, ref statement);

            var parameters = new ExpressionBuilderParams(context, scope, metaWriter, pinCodeWriter, null);

            var result = expressionBuilder.CreateExpression(parameters, statement);

            return expressionBuilder.PinExpressionToVariable(parameters, null, result);
        }

        public override ExpressionResult GetExpression(ExpressionBuilderParams p,
            EvaluationStatement statement) =>
            CreateBatchExpression(p, statement);

        public override ExpressionResult GetExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext,
            EvaluationStatement statement) =>
            CreateBatchExpression(
                new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, usageContext), statement);

        public override ExpressionResult GetConditionalExpression(ExpressionBuilderParams p,
            EvaluationStatement statement) => CreateBatchConditionalExpression(p, statement);

        public override ExpressionResult GetConditionalExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext,
            EvaluationStatement statement) =>
            CreateBatchConditionalExpression(
                new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, usageContext), statement);

        public override ExpressionResult CallApiFunction<TApiFunc>(ExpressionBuilderParams p,
            EvaluationStatement[] parameters, IStatement parentStatement, StatementInfo statementInfo)
        {
            var nParams = new List<EvaluationStatement>(parameters.Length);
            foreach (var param in parameters)
            {
                nParams.Add(ProcessEvaluation(p.Context, p.Scope, param));
            }

            return BatchDefaultExpressionBuilder.Instance.CallApiFunction<TApiFunc>(p, nParams.ToArray(),
                parentStatement, statementInfo);
        }
    }
}