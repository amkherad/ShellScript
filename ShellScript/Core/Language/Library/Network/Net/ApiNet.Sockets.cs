using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.Network.Net
{
    public partial class ApiNet
    {
        public abstract class GetOpenSockets : ApiBaseFunction
        {
            public override string Name => nameof(GetOpenSockets);
            public override string Summary =>
                "Returns lines describing open TCP/UDP sockets (state, addresses, process) from the OS.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetSocketProcessId : ApiBaseFunction
        {
            public override string Name => nameof(GetSocketProcessId);
            public override string Summary =>
                "Returns the owning process id parsed from a socket line (ss or netstat), or 0 when unknown.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "SocketLine", null, null)
            };
        }
    }
}
