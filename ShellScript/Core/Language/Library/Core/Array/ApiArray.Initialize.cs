using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Array
{
    public partial class ApiArray
    {
        public abstract class Initialize : ApiBaseFunction
        {
            public override string Name => nameof(Initialize);
            public override string Summary => "Initializes an array with zeros.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Array, "Array", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "Length", null, null),
            };
        }
    }
}