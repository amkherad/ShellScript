using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Text
{
    public abstract partial class ApiRegex : ApiBaseClass
    {
        public const string ClassAccessName = "Regex";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class IsMatch : ApiBaseFunction
        {
            public override string Name => nameof(IsMatch);
            public override string Summary => "Checks whether input matches a regular expression.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Input", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Pattern", null, null)
            };
        }

        public abstract class Replace : ApiBaseFunction
        {
            public override string Name => nameof(Replace);
            public override string Summary => "Replaces regex matches in input.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Input", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Pattern", null, null),
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Replacement", null, null)
            };
        }
    }
}
