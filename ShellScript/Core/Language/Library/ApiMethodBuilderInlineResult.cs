using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.Library
{
    public class ApiMethodBuilderInlineResult : IApiMethodBuilderResult
    {
        public EvaluationStatement Statement { get; }

        public ApiMethodBuilderInlineResult(EvaluationStatement statement)
        {
            Statement = statement;
        }
    }
}