using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Diagnostics
{
    public abstract partial class ApiLog : ApiBaseClass
    {
        public const string ClassAccessName = "Log";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class Info : ApiBaseFunction
        {
            public override string Name => nameof(Info);
            public override string Summary => "Writes an info message to stderr.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }

        public abstract class Warn : ApiBaseFunction
        {
            public override string Name => nameof(Warn);
            public override string Summary => "Writes a warning to stderr.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }

        public abstract class Error : ApiBaseFunction
        {
            public override string Name => nameof(Error);
            public override string Summary => "Writes an error to stderr.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }

        public abstract class Debug : ApiBaseFunction
        {
            public override string Name => nameof(Debug);
            public override string Summary => "Writes a debug message to stderr.";
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
