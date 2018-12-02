using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Array
{
    public partial class ApiArray
    {
        public abstract class Copy : ApiBaseFunction
        {
            public override string Name => nameof(Copy);
            public override string Summary => "Copies an array to another one.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Array, "Destination", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Array, "Source", null, null),
            };
        }
    }
}