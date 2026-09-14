using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Data.Yaml
{
    public abstract partial class ApiYaml : ApiBaseClass
    {
        public const string ClassAccessName = "Yaml";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetPath : ApiBaseFunction
        {
            public override string Name => nameof(GetPath);
            public override string Summary => "Reads a value from YAML using a dotted path.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "YamlText", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Path", null, null)
            };
        }

        public abstract class IsValid : ApiBaseFunction
        {
            public override string Name => nameof(IsValid);
            public override string Summary => "Checks whether text is valid YAML.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "YamlText", null, null)
            };
        }
    }
}
