using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Environment
{
    public partial class ApiEnvironment
    {
        public abstract class SetVariable : ApiBaseFunction
        {
            public override string Name => nameof(SetVariable);
            public override string Summary => "Sets an environment variable for the current process.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                NameParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value", null, null)
            };
        }
    }
}
