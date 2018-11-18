using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Convert
{
    public partial class ApiConvert
    {
        public abstract class ToFloat : ApiBaseFunction
        {
            public override string Name => nameof(ToFloat);
            public override string Summary { get; }
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Float;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Value", null, null, true)
            };
        }
    }
}