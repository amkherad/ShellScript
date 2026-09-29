using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.System.Thread
{
    public abstract partial class ApiThread
    {
        public abstract class Sleep : ApiBaseFunction
        {
            public override string Name => nameof(Sleep);
            public override string Summary => "Suspends the current thread for the given number of milliseconds.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "Milliseconds", null, null)
            };
        }
    }
}
