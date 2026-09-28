using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Network.Net
{
    public abstract partial class ApiNet : ApiBaseClass
    {
        public const string ClassAccessName = "Net";
        public override string Name => "Net";

        public static readonly FunctionParameterDefinitionStatement EndpointParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Endpoint", null, null);

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}