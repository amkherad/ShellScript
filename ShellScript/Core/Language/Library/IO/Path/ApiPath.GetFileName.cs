using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public abstract partial class ApiPath
    {
        public abstract class GetFileName : ApiBaseFunction
        {
            public override string Name => nameof(GetFileName);
            public override string Summary => "Returns the final component of a path.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = new[] { PathParameter };
        }
    }
}
