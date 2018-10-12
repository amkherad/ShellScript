using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform
    {
        public class IsSuperUser : TestFunction
        {
            public override string Name => nameof(IsSuperUser);
            public override string Summary { get; }
            public override string ClassName => ClassAccessName;
            public override DataTypes DataType => DataTypes.Boolean;


            public override FunctionParameterDefinitionStatement[] Parameters { get; }


            protected override ExpressionResult CreateTestExpression(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return new ExpressionResult(
                    DataType,
                    $"$(whoami) == 'root'",
                    functionCallStatement
                );
            }
        }
    }
}