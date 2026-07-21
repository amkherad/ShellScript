using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Network.Net
{
    public partial class ApiNet
    {
        public abstract class Ping : ApiBaseFunction
        {
            public override string Name => nameof(Ping);
            public override string Summary => "Checks whether an endpoint responds to one ICMP echo request.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                EndpointParameter
            };
        }
    }
}
