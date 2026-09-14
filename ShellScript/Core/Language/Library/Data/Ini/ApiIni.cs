using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Data.Ini
{
    public abstract partial class ApiIni : ApiBaseClass
    {
        public const string ClassAccessName = "Ini";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetValue : ApiBaseFunction
        {
            public override string Name => nameof(GetValue);
            public override string Summary => "Reads a value from an INI file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Section", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Key", null, null)
            };
        }

        public abstract class SetValue : ApiBaseFunction
        {
            public override string Name => nameof(SetValue);
            public override string Summary => "Sets a value in an INI file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Section", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Key", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value", null, null)
            };
        }
    }
}
