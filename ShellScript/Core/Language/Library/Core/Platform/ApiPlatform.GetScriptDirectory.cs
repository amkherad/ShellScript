using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Platform
{
    public abstract partial class ApiPlatform
    {
        public abstract class GetScriptDirectory : ApiBaseFunction
        {
            public override string Name => nameof(GetScriptDirectory);
            public override string Summary =>
                "Returns the absolute directory containing the running script (Unix-Bash: directory of the compiled .bash file).";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }
    }
}
