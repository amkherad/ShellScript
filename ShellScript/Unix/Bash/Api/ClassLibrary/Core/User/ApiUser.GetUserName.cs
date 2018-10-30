using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.User
{
    public partial class ApiUser
    {
        public class GetUserName : TestFunction
        {
            public override string Name => nameof(GetUserName);
            public override string Summary { get; }
            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;


            public override FunctionParameterDefinitionStatement[] Parameters { get; }


            protected override ExpressionResult CreateTestExpression(ExpressionBuilderParams p,
                FunctionCallStatement functionCallStatement)
            {
                return new ExpressionResult(
                    TypeDescriptor,
                    "`whoami`",
                    functionCallStatement
                );
            }
        }
    }
}