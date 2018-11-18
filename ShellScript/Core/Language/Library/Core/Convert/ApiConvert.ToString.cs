using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Convert
{
    public partial class ApiConvert
    {
        public abstract class ToString : ApiBaseFunction
        {
            public override string Name => nameof(ToString);
            public override string Summary { get; }
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Value", null, null, true)
            };
        }
    }
}