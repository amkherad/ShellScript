using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Diagnostics
{
    public abstract partial class ApiConsole
    {
        public abstract class MoveCursorHome : ApiBaseFunction
        {
            public override string Name => nameof(MoveCursorHome);
            public override string Summary => "Moves the cursor to the top-left without clearing the screen.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class BeginBatchWrite : ApiBaseFunction
        {
            public override string Name => nameof(BeginBatchWrite);
            public override string Summary =>
                "Redirects subsequent stdout writes to an in-memory buffer until EndBatchWrite (reduces TUI flicker).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class EndBatchWrite : ApiBaseFunction
        {
            public override string Name => nameof(EndBatchWrite);
            public override string Summary => "Flushes the batch buffer to the terminal in one write.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class Clear : ApiBaseFunction
        {
            public override string Name => nameof(Clear);
            public override string Summary => "Clears the terminal screen and moves the cursor to the home position.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class IsTerminal : ApiBaseFunction
        {
            public override string Name => nameof(IsTerminal);
            public override string Summary => "Returns whether standard output is connected to an interactive terminal.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetWindowWidth : ApiBaseFunction
        {
            public override string Name => nameof(GetWindowWidth);
            public override string Summary => "Returns terminal width in columns.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetWindowHeight : ApiBaseFunction
        {
            public override string Name => nameof(GetWindowHeight);
            public override string Summary => "Returns terminal height in rows.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class ReadKeyTimeout : ApiBaseFunction
        {
            public override string Name => nameof(ReadKeyTimeout);
            public override string Summary =>
                "Reads a single key when available before the timeout; arrow keys return UP, DOWN, LEFT, or RIGHT; returns empty when no key was pressed.";
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

        public abstract class EnterInteractiveInputMode : ApiBaseFunction
        {
            public override string Name => nameof(EnterInteractiveInputMode);
            public override string Summary =>
                "Puts stdin in non-canonical, no-echo mode for TUI input (restored by ExitInteractiveInputMode). Ctrl+C sets a flag read by ConsumeInterruptRequest.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class ExitInteractiveInputMode : ApiBaseFunction
        {
            public override string Name => nameof(ExitInteractiveInputMode);
            public override string Summary => "Restores stdin tty settings saved by EnterInteractiveInputMode.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class ConsumeInterruptRequest : ApiBaseFunction
        {
            public override string Name => nameof(ConsumeInterruptRequest);
            public override string Summary =>
                "Returns true once after Ctrl+C (SIGINT) or SIGTERM while in interactive input mode; clears the flag.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}
