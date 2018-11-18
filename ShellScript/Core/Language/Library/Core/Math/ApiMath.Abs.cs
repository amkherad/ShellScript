using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Math
{
    public abstract partial class ApiMath
    {
        public abstract class Abs : ApiBaseFunction
        {
            private const string ApiMathAbsBashMethodName = "Abs_";

            public override string Name => nameof(Abs);
            public override string Summary => "Returns the absolute value of the number.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;

            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                NumberParameter
            };
        }
    }
}