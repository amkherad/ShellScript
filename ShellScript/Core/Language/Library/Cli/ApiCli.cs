using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Cli
{
    public abstract partial class ApiCli : ApiBaseClass
    {
        public const string ClassAccessName = "Cli";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetArgumentCount : ApiBaseFunction
        {
            public override string Name => nameof(GetArgumentCount);
            public override string Summary => "Returns script argument count.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Integer;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetArgument : ApiBaseFunction
        {
            public override string Name => nameof(GetArgument);
            public override string Summary => "Returns argument at zero-based index.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.Integer, "Index", null, null)
            };
        }

        public abstract class HasFlag : ApiBaseFunction
        {
            public override string Name => nameof(HasFlag);
            public override string Summary => "Checks whether a flag exists in script arguments.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Flag", null, null)
            };
        }

        public abstract class GetFlagValue : ApiBaseFunction
        {
            public override string Name => nameof(GetFlagValue);
            public override string Summary => "Returns value after --flag= or next argument.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Flag", null, null)
            };
        }
    }
}
