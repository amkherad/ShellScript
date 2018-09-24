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
        public static (DataTypes, string, EvaluationStatement) CreateBashExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(context, scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(context, scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashStringConcatenationExpressionBuilder.Instance
                : BashDefaultExpressionBuilder.Instance;

            var parameters = new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, usageContext);

            return expressionBuilder.CreateExpression(parameters, evalStt);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (DataTypes, string, EvaluationStatement) CreateBashConditionalExpression(Context context, Scope scope,
            TextWriter metaWriter, TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement evalStt)
        {
            evalStt = ProcessEvaluation(context, scope, evalStt);

            var structure = StatementHelpers.TraverseTreeAndGetExpressionTypes(context, scope, evalStt);

            IExpressionBuilder expressionBuilder = structure.Types.Contains(DataTypes.String)
                ? BashConditionalStringConcatenationExpressionBuilder.Instance
                : BashConditionalExpressionBuilder.Instance;

            var parameters = new ExpressionBuilderParams(context, scope, metaWriter, nonInlinePartWriter, usageContext);

            return expressionBuilder.CreateExpression(parameters, evalStt);
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

            var expression = CreateBashExpression(context, scope, metaWriter, nonInlinePartWriter, null, evalStt);

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

            var (dataType, expression, template) = expressionBuilder.CreateExpression(parameters, statement);

            return expressionBuilder.PinExpressionToVariable(parameters, dataType, null, expression, statement);
        }

        public override (DataTypes, string, EvaluationStatement) GetInline(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement) =>
            CreateBashExpression(context, scope, metaWriter, nonInlinePartWriter, usageContext, statement);

        public override (DataTypes, string, EvaluationStatement) GetInlineConditional(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement) =>
            CreateBashConditionalExpression(context, scope, metaWriter, nonInlinePartWriter, usageContext, statement);
    }
}