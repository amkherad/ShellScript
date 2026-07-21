using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Environment
{
    public abstract partial class ApiEnvironment
    {
        public abstract class GetHomeDirectory : ApiBaseFunction
        {
            public override string Name => nameof(GetHomeDirectory);
            public override string Summary => "Returns the current user home directory.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = new FunctionParameterDefinitionStatement[0];
        }
    }
}
