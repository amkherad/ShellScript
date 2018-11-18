using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.File
{
    public partial class ApiFile
    {
        public abstract class IsLink : ApiBaseFunction
        {
            public override string Name => "IsLink";
            public override string Summary => "Checks whether a file exists and is a link to another file.";
            public override string ClassName => ClassAccessName;
            public override bool IsStatic => true;
            
            public override TypeDescriptor TypeDescriptor => TypeDescriptor.Boolean;
            
            public override FunctionParameterDefinitionStatement[] Parameters { get; } =
            {
                FilePathParameter
            };
        }
    }
}