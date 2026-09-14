using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.System.Process
{
    public abstract partial class ApiProcess : ApiBaseClass
    {
        public const string ClassAccessName = "Process";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetCurrentId : ApiBaseFunction
        {
            public override string Name => nameof(GetCurrentId);
            public override string Summary => "Returns the current process id.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetParentId : ApiBaseFunction
        {
            public override string Name => nameof(GetParentId);
            public override string Summary => "Returns the parent process id.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class Exists : ApiBaseFunction
        {
            public override string Name => nameof(Exists);
            public override string Summary => "Checks whether a process id exists.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "ProcessId", null, null)
            };
        }

        public abstract class Kill : ApiBaseFunction
        {
            public override string Name => nameof(Kill);
            public override string Summary => "Sends SIGTERM to a process.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "ProcessId", null, null)
            };
        }

        public abstract class Run : ApiBaseFunction
        {
            public override string Name => nameof(Run);
            public override string Summary => "Runs a shell command.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Command", null, null)
            };
        }

        public abstract class RunAndCapture : ApiBaseFunction
        {
            public override string Name => nameof(RunAndCapture);
            public override string Summary => "Runs a command and returns combined stdout.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Command", null, null)
            };
        }
    }
}
