using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public partial class ApiPath
    {
        public abstract class GetTempPath : ApiBaseFunction
        {
            public override string Name => nameof(GetTempPath);
            public override string Summary => "Returns the directory for temporary files.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.String;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
                new FunctionParameterDefinitionStatement[0];
        }

        public abstract class IsPathRooted : ApiBaseFunction
        {
            public override string Name => nameof(IsPathRooted);
            public override string Summary => "Checks whether a path is absolute.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            public override FunctionParameterDefinitionStatement[] Parameters { get; } = {PathParameter};
        }
    }
}
