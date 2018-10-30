using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public abstract class TestCommandBase : TestFunction
    {
        public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;

        private string _testCharacter;

        protected TestCommandBase(string testCharacter)
        {
            _testCharacter = testCharacter;
        }

        public override FunctionParameterDefinitionStatement[] Parameters { get; } =
        {
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null),
        };

        protected override ExpressionResult CreateTestExpression(ExpressionBuilderParams p,
            FunctionCallStatement functionCallStatement)
        {
            var parameter = functionCallStatement.Parameters[0];

            var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
            var result = transpiler.GetExpression(p.Context, p.Scope, p.MetaWriter, p.NonInlinePartWriter, null,
                parameter);

            return new ExpressionResult(
                TypeDescriptor,
                $"[ -{_testCharacter} {result.Expression} ]",
                functionCallStatement
            );
        }
    }
}