using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Data.DotEnv
{
    public abstract partial class ApiDotEnv : ApiBaseClass
    {
        public const string ClassAccessName = "DotEnv";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class Load : ApiBaseFunction
        {
            public override string Name => nameof(Load);
            public override string Summary => "Loads KEY=VALUE pairs from a .env file into the environment.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null)
            };
        }

        public abstract class Get : ApiBaseFunction
        {
            public override string Name => nameof(Get);
            public override string Summary => "Reads a variable from a .env file without exporting it.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Key", null, null)
            };
        }
    }
}
