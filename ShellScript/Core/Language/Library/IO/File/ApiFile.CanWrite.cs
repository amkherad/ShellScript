using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.File
{
    public partial class ApiFile
    {
        public abstract class CanWrite : ApiBaseFunction
        {
            public override string Name => "CanWrite";
            public override string Summary => "Checks whether a file has write permission.";
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