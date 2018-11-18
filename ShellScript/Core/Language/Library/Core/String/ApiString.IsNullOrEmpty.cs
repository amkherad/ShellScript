using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiString
    {
        public abstract class IsNullOrEmpty : ApiBaseFunction
        {
            public override string Name => nameof(IsNullOrEmpty);
            public override string Summary { get; }
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