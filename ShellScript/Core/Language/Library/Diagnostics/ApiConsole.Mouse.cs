using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Diagnostics
{
    public abstract partial class ApiConsole
    {
        public abstract class IsStdinTerminal : ApiBaseFunction
        {
            public override string Name => nameof(IsStdinTerminal);
            public override string Summary =>
                "Returns whether standard input is connected to an interactive terminal (required for keyboard/mouse input).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class EnableMouseReporting : ApiBaseFunction
        {
            public override string Name => nameof(EnableMouseReporting);
            public override string Summary =>
                "Enables SGR click/wheel mouse reporting (use with EnterInteractiveInputMode). Pair with DisableMouseReporting on exit.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class DisableMouseReporting : ApiBaseFunction
        {
            public override string Name => nameof(DisableMouseReporting);
            public override string Summary => "Disables mouse reporting enabled by EnableMouseReporting.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class ReadTerminalInputTimeout : ApiBaseFunction
        {
            public override string Name => nameof(ReadTerminalInputTimeout);
            public override string Summary =>
                "Reads a key or mouse event before the timeout. Returns an empty string when idle. " +
                "Arrow keys return UP, DOWN, LEFT, or RIGHT; Page Up/Down return PAGE_UP and PAGE_DOWN; bare Escape returns ESC. " +
                "Mouse events use MOUSE:row:column:button (SGR wheel buttons 64/65).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "TimeoutMilliseconds", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Boolean, "echoKey",
                    new ConstantValueStatement(TypeDescriptor.Boolean, "false", null), null)
            };
        }
    }
}
