using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class EndsWith : ApiBaseFunction
        {
            public override string Name => nameof(EndsWith);
            public override string Summary => "Checks whether a string ends with a value.";
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
