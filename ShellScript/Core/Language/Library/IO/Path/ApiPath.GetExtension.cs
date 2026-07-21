using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public abstract partial class ApiPath
    {
        public abstract class GetExtension : ApiBaseFunction
        {
            public override string Name => nameof(GetExtension);
            public override string Summary => "Returns the final extension including the leading dot, or an empty string.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = new[] { PathParameter };
        }
    }
}
