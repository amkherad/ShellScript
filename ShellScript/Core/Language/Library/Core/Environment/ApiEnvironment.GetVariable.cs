using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Environment
{
    public abstract partial class ApiEnvironment
    {
        public abstract class GetVariable : ApiBaseFunction
        {
            public override string Name => nameof(GetVariable);
            public override string Summary => "Returns the value of an environment variable or an empty string when it is unset.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = new[] { NameParameter };
        }
    }
}
