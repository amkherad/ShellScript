using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.System.Thread
{
    public abstract partial class ApiThread : ApiBaseClass
    {
        public const string ClassAccessName = "Thread";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetCurrentId : ApiBaseFunction
        {
            public override string Name => nameof(GetCurrentId);
            public override string Summary => "Returns current thread identifier (process id in bash).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}
