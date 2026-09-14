using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Network.Net
{
    public partial class ApiNet
    {
        public abstract class Download : ApiBaseFunction
        {
            public override string Name => nameof(Download);
            public override string Summary => "Downloads a URL to a file path.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Url", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "DestinationPath", null, null)
            };
        }

        public abstract class HttpGet : ApiBaseFunction
        {
            public override string Name => nameof(HttpGet);
            public override string Summary => "Performs an HTTP GET and returns the response body.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Url", null, null)
            };
        }

        public abstract class ResolveHost : ApiBaseFunction
        {
            public override string Name => nameof(ResolveHost);
            public override string Summary => "Resolves a host name to an address.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "HostName", null, null)
            };
        }
    }
}
