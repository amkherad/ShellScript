using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public abstract partial class ApiPath
    {
        public abstract class Combine : ApiBaseFunction
        {
            public override string Name => nameof(Combine);
            public override string Summary => "Combines two path components with one slash.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = new[] { LeftParameter, RightParameter };
        }
    }
}
