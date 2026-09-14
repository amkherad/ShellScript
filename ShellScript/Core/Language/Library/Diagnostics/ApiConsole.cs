using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Diagnostics
{
    public abstract partial class ApiConsole : ApiBaseClass
    {
        public const string ClassAccessName = "Console";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class WriteLine : ApiBaseFunction
        {
            public override string Name => nameof(WriteLine);
            public override string Summary => "Writes a line to stdout.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }

        public abstract class WriteError : ApiBaseFunction
        {
            public override string Name => nameof(WriteError);
            public override string Summary => "Writes a line to stderr.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }
    }
}
