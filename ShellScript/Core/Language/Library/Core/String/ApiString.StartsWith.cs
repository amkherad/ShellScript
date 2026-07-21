using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class StartsWith : ApiBaseFunction
        {
            public override string Name => nameof(StartsWith);
            public override string Summary => "Checks whether a string starts with a value.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Search", null, null),
            };
        }
    }
}
