using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling.ExpressionBuilders;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Core.Language.Library.Core.User
{
    public partial class ApiUser
    {
        public abstract class GetUserName : ApiBaseFunction
        {
            public override string Name => nameof(GetUserName);
            public override string Summary { get; }
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;

            public override FunctionParameterDefinitionStatement[] Parameters { get; }
        }
    }
}