using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Diagnostics
{
    public abstract partial class ApiConsole
    {
        public abstract class Write : ApiBaseFunction
        {
            public override string Name => nameof(Write);
            public override string Summary => "Writes text to stdout without a trailing newline.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }

        public abstract class SetCursorPosition : ApiBaseFunction
        {
            public override string Name => nameof(SetCursorPosition);
            public override string Summary =>
                "Moves the cursor to a 1-based row and column (ANSI), for TUI overlays and dialogs.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "Row", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "Column", null, null)
            };
        }
    }
}
