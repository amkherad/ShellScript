using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Library.Network.Net
{
    public partial class ApiNet
    {
        public abstract class GetNetworkInterfaces : ApiBaseFunction
        {
            public override string Name => nameof(GetNetworkInterfaces);
            public override string Summary => "Returns network interface names on the local machine.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetOpenSocketsOnInterface : ApiBaseFunction
        {
            public override string Name => nameof(GetOpenSocketsOnInterface);
            public override string Summary =>
                "Returns open socket lines bound to the given interface (empty name lists all).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor =>
                new TypeDescriptor(DataTypes.String | DataTypes.Array);
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "InterfaceName", null, null)
            };
        }
    }
}
