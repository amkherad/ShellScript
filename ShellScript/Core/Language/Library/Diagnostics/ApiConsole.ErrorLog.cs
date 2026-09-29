using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Diagnostics
{
    public abstract partial class ApiConsole
    {
        public abstract class EnableErrorLog : ApiBaseFunction
        {
            public override string Name => nameof(EnableErrorLog);
            public override string Summary =>
                "Mirrors subsequent Console.WriteError output to a file (stderr is still written). Must be called explicitly.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Boolean, "Append",
                    new ConstantValueStatement(TypeDescriptor.Boolean, "true", null), null)
            };
        }

        public abstract class DisableErrorLog : ApiBaseFunction
        {
            public override string Name => nameof(DisableErrorLog);
            public override string Summary => "Stops mirroring Console.WriteError to the file set by EnableErrorLog.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}
