using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Testing
{
    public abstract partial class ApiAssert : ApiBaseClass
    {
        public const string ClassAccessName = "Assert";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public new abstract class Equals : ApiBaseFunction
        {
            public override string Name => nameof(Equals);
            public override string Summary => "Fails when the expected and actual values are not equal.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Expected", null, null, true),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Actual", null, null, true),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class NotEquals : ApiBaseFunction
        {
            public override string Name => nameof(NotEquals);
            public override string Summary => "Fails when the expected and actual values are equal.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Expected", null, null, true),
                new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Actual", null, null, true),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class True : ApiBaseFunction
        {
            public override string Name => nameof(True);
            public override string Summary => "Fails when the condition is false.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Boolean, "Condition", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class False : ApiBaseFunction
        {
            public override string Name => nameof(False);
            public override string Summary => "Fails when the condition is true.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Boolean, "Condition", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class Fail : ApiBaseFunction
        {
            public override string Name => nameof(Fail);
            public override string Summary => "Unconditionally fails the test with a message.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Message", null, null)
            };
        }
    }
}
