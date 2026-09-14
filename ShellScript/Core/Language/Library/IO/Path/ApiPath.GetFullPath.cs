using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public partial class ApiPath
    {
        public abstract class GetFullPath : ApiBaseFunction
        {
            public override string Name => nameof(GetFullPath);
            public override string Summary => "Returns the absolute path.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {PathParameter};
        }
    }
}
