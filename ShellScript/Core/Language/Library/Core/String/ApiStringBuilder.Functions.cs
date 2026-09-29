using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public partial class ApiStringBuilder
    {
        public abstract class Create : ApiBaseFunction
        {
            public override string Name => nameof(Create);
            public override string Summary =>
                "Creates a new string builder (empty string buffer). Pass an optional initial value.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Initial",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class Append : ApiBaseFunction
        {
            public override string Name => nameof(Append);
            public override string Summary => "Appends text to the builder (mutates the builder variable).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                BuilderParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value", null, null)
            };
        }

        public abstract class AppendLine : ApiBaseFunction
        {
            public override string Name => nameof(AppendLine);
            public override string Summary =>
                "Appends text and a newline to the builder; omit Value to append only a newline.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                BuilderParameter,
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Value",
                    new ConstantValueStatement(TypeDescriptor.String, "", null), null)
            };
        }

        public abstract class Clear : ApiBaseFunction
        {
            public override string Name => nameof(Clear);
            public override string Summary => "Clears the builder contents.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Void;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {BuilderParameter};
        }

        public new abstract class ToString : ApiBaseFunction
        {
            public override string Name => nameof(ToString);
            public override string Summary => "Returns the current builder text.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {BuilderParameter};
        }

        public abstract class GetLength : ApiBaseFunction
        {
            public override string Name => nameof(GetLength);
            public override string Summary => "Returns the current length of the builder text.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {BuilderParameter};
        }
    }
}
