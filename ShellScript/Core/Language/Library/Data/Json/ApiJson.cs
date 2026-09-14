using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Data.Json
{
    public abstract partial class ApiJson : ApiBaseClass
    {
        public const string ClassAccessName = "Json";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class IsValid : ApiBaseFunction
        {
            public override string Name => nameof(IsValid);
            public override string Summary => "Checks whether text is valid JSON.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "JsonText", null, null)
            };
        }

        public abstract class GetPath : ApiBaseFunction
        {
            public override string Name => nameof(GetPath);
            public override string Summary => "Reads a value using a jq-style path.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "JsonText", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Path", null, null)
            };
        }

        public abstract class PrettyPrint : ApiBaseFunction
        {
            public override string Name => nameof(PrettyPrint);
            public override string Summary => "Formats JSON text.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "JsonText", null, null)
            };
        }
    }
}
