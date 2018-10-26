using System.IO;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.CompilerServices.Transpiling
{
    public interface IPlatformEvaluationStatementTranspiler : IPlatformStatementTranspiler
    {
        PinnedVariableResult PinEvaluationToVariable(Context context, Scope scope, TextWriter metaWriter,
            TextWriter pinCodeWriter, EvaluationStatement statement);

        ExpressionResult GetExpression(ExpressionBuilderParams p, EvaluationStatement statement);

        ExpressionResult GetExpression(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement);

        ExpressionResult GetConditionalExpression(ExpressionBuilderParams p, EvaluationStatement statement);

        ExpressionResult GetConditionalExpression(Context context, Scope scope, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement usageContext, EvaluationStatement statement);
    }
}