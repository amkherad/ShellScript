using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Math
{
    public abstract partial class ApiMath
    {
        public abstract class Min : ApiBaseFunction
        {
            public override string Name => nameof(Min);
            public override string Summary => "Returns the smaller of two numbers.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                NumberParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Numeric, "Second", null, null)
            };
        }

        public abstract class Max : ApiBaseFunction
        {
            public override string Name => nameof(Max);
            public override string Summary => "Returns the larger of two numbers.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                NumberParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Numeric, "Second", null, null)
            };
        }

        public abstract class Floor : ApiBaseFunction
        {
            public override string Name => nameof(Floor);
            public override string Summary => "Returns the largest integral value less than or equal to the number.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {NumberParameter};
        }

        public abstract class Ceiling : ApiBaseFunction
        {
            public override string Name => nameof(Ceiling);
            public override string Summary => "Returns the smallest integral value greater than or equal to the number.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {NumberParameter};
        }

        public abstract class Round : ApiBaseFunction
        {
            public override string Name => nameof(Round);
            public override string Summary => "Rounds a number to the nearest integral value.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {NumberParameter};
        }

        public abstract class Truncate : ApiBaseFunction
        {
            public override string Name => nameof(Truncate);
            public override string Summary => "Returns the integral part of a number.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {NumberParameter};
        }

        public abstract class Sqrt : ApiBaseFunction
        {
            public override string Name => nameof(Sqrt);
            public override string Summary => "Returns the square root of a number.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {NumberParameter};
        }

        public abstract class Pow : ApiBaseFunction
        {
            public override string Name => nameof(Pow);
            public override string Summary => "Returns a number raised to the specified power.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Numeric;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                NumberParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Numeric, "Exponent", null, null)
            };
        }

        public abstract class Sign : ApiBaseFunction
        {
            public override string Name => nameof(Sign);
            public override string Summary => "Returns -1, 0, or 1 depending on the sign of the number.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {NumberParameter};
        }
    }
}
