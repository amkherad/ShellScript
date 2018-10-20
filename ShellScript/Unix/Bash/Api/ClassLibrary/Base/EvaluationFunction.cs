using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    /// <summary>
    /// Represents simple functions with input parameters of primitive types and return value of primitive type.
    /// And need $
    /// </summary>
    public abstract class EvaluationFunction : ApiBaseFunction
    {
        public override bool IsStatic => true;
        public override bool AllowDynamicParams => false;
        
        protected abstract ExpressionResult CreateEvaluationExpression(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement);

        public override IApiMethodBuilderResult Build(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            AssertParameters(functionCallStatement.Parameters);

            var result = CreateEvaluationExpression(p, functionCallStatement);

            return new ApiMethodBuilderRawResult(result);
        }
    }
}