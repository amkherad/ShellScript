using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public abstract partial class ApiPath
    {
        public abstract class GetDirectoryName : ApiBaseFunction
        {
            public override string Name => nameof(GetDirectoryName);
            public override string Summary => "Returns the directory component of a path.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = new[] { PathParameter };
        }
    }
}
