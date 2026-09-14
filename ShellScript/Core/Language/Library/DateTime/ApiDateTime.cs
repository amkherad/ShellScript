using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.DateTime
{
    public abstract partial class ApiDateTime : ApiBaseClass
    {
        public const string ClassAccessName = "DateTime";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class Now : ApiBaseFunction
        {
            public override string Name => nameof(Now);
            public override string Summary => "Returns local time formatted string.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class UtcNow : ApiBaseFunction
        {
            public override string Name => nameof(UtcNow);
            public override string Summary => "Returns UTC time formatted string.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class Format : ApiBaseFunction
        {
            public override string Name => nameof(Format);
            public override string Summary => "Formats Unix seconds with a date format string.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "UnixSeconds", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Format", null, null)
            };
        }

        public abstract class ToUnixTime : ApiBaseFunction
        {
            public override string Name => nameof(ToUnixTime);
            public override string Summary => "Returns current Unix timestamp in seconds.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}
