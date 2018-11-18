using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class IsNullOrWhiteSpace : ApiBaseFunction
        {
            public override string Name => nameof(IsNullOrWhiteSpace);
            public override string Summary => "Checks whether a string is null or only contains spaces.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                StringParameter
            };
        }
    }
}