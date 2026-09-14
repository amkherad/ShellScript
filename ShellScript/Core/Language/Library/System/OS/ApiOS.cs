using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.System.OS
{
    public abstract partial class ApiOS : ApiBaseClass
    {
        public const string ClassAccessName = "OS";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];

        public abstract class GetHostName : ApiBaseFunction
        {
            public override string Name => nameof(GetHostName);
            public override string Summary => "Returns the system host name.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetKernelName : ApiBaseFunction
        {
            public override string Name => nameof(GetKernelName);
            public override string Summary => "Returns the kernel name (e.g. Linux).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetArchitecture : ApiBaseFunction
        {
            public override string Name => nameof(GetArchitecture);
            public override string Summary => "Returns machine hardware name.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class GetOsVersion : ApiBaseFunction
        {
            public override string Name => nameof(GetOsVersion);
            public override string Summary => "Returns operating system version text.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}
