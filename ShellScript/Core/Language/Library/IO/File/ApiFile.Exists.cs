using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.File
{
    public partial class ApiFile
    {
        public abstract class Exists : ApiBaseFunction
        {
            public override string Name => "Exists";
            public override string Summary => "Checks whether a file exists.";
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