using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public abstract class FileTestBase : TestFunction
        {
            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Boolean;

            public override bool IsStatic => true;
            public override bool AllowDynamicParams => false;

            private string _testCharacter;
            
            protected FileTestBase(string testCharacter)
            {
                _testCharacter = testCharacter;
            }

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(DataTypes.String, "FilePath", null, null),
            };

            protected override (DataTypes, string, EvaluationStatement) CreateTestExpression(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                var parameter = functionCallStatement.Parameters[0];

                var transpiler = p.Context.GetEvaluationTranspilerForStatement(parameter);
                var (dataType, expression, template) = transpiler.GetExpression(p.Context, p.Scope, p.MetaWriter,
                    p.NonInlinePartWriter, null, parameter);

                return (DataType, $"[ -{_testCharacter} {expression} ]", functionCallStatement);
            }
        }
    }
}