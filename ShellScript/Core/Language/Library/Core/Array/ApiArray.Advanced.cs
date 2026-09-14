using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Array
{
    public partial class ApiArray
    {
        public abstract class IndexOf : ApiBaseFunction
        {
            public override string Name => nameof(IndexOf);
            public override string Summary => "Returns index of value or -1.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                ArrayParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Value", null, null, true)
            };
        }

        public abstract class Contains : ApiBaseFunction
        {
            public override string Name => nameof(Contains);
            public override string Summary => "Checks whether array contains a value.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                ArrayParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Value", null, null, true)
            };
        }

        public abstract class Clear : ApiBaseFunction
        {
            public override string Name => nameof(Clear);
            public override string Summary => "Removes all elements from an array.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {ArrayParameter};
        }

        public abstract class Reverse : ApiBaseFunction
        {
            public override string Name => nameof(Reverse);
            public override string Summary => "Reverses an array in place.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {ArrayParameter};
        }

        public abstract class Add : ApiBaseFunction
        {
            public override string Name => nameof(Add);
            public override string Summary => "Appends a value to an array.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                ArrayParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Value", null, null, true)
            };
        }
    }
}
