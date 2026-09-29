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
            public override string Summary =>
                "Writes a line to stderr. Also appends to the file set by EnableErrorLog when that feature is enabled.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }

        public abstract class ReadLine : ApiBaseFunction
        {
            public override string Name => nameof(ReadLine);
            public override string Summary => "Reads a line of text from standard input (no trailing newline).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class ReadText : ApiBaseFunction
        {
            public override string Name => nameof(ReadText);
            public override string Summary => "Writes an optional prompt, then reads a line of text from standard input.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Prompt",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class ReadKey : ApiBaseFunction
        {
            public override string Name => nameof(ReadKey);
            public override string Summary =>
                "Reads a single key from the terminal without requiring Enter (echoKey shows the key as typed).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Boolean, "echoKey",
                    new ConstantValueStatement(TypeDescriptor.Boolean, "false", null), null)
            };
        }
    }
}
