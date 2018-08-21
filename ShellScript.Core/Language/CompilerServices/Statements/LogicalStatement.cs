using ShellScript.Core.Language.Sdk;

namespace ShellScript.Core.Language.CompilerServices.Statements
{
    public abstract class LogicalStatement : EvaluationStatement
    {
        public override bool IsBlockStatement => false;

        public static IStatement FromEvaluationStatement(EvaluationStatement statement)
        {
            if (statement is LogicalStatement logical)
            {
                return logical;
            }

            return new FunctionCallStatement(KeywordLikeFunctions.CastToBoolean, new[] {statement});
        }
    }
}