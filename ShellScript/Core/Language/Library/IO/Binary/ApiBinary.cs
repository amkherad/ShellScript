using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Binary
{
    public abstract partial class ApiBinary : ApiBaseClass
    {
        public const string ClassAccessName = "Binary";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class ToBase64 : ApiBaseFunction
        {
            public override string Name => nameof(ToBase64);
            public override string Summary => "Encodes raw bytes (string) to base64.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Data", null, null)
            };
        }

        public abstract class FromBase64 : ApiBaseFunction
        {
            public override string Name => nameof(FromBase64);
            public override string Summary => "Decodes base64 to a raw string.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Base64Data", null, null)
            };
        }
    }
}
