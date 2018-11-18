using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Core.Language.Library.Network.Net
{
    public partial class ApiNet
    {
        public abstract class Ping : ApiBaseFunction
        {
            public override string Name => nameof(Ping);

            public override string Summary =>
                "Takes a string and execute it as a void-result platform-dependent shell command.";

            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;

            public override bool IsStatic => true;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Endpoint", null, null),
            };
        }
    }
}