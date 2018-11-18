using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Platform
{
    public abstract partial class ApiPlatform
    {
        public abstract class Call : ApiBaseFunction
        {
            public override string Name => nameof(Call);

            public override string Summary =>
                "Takes a string and execute it as a void-result platform-dependent shell command.";

            public override string ClassName => ClassAccessName;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;

            public override bool IsStatic => true;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "RawCommand", null, null),
            };
        }
    }
}