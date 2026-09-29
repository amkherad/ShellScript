using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.System.Process
{
    public partial class ApiProcess
    {
        public abstract class GetName : ApiBaseFunction
        {
            public override string Name => nameof(GetName);
            public override string Summary => "Returns the short command name for a process id, or empty when unknown.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "ProcessId", null, null)
            };
        }
    }
}
