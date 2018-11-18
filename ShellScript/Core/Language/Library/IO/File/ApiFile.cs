using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.File
{
    public abstract partial class ApiFile : ApiBaseClass
    {
        public const string ClassAccessName = "File";
        public override string Name => "File";

        public override IApiVariable[] Variables => new IApiVariable[0];
        
        public static readonly FunctionParameterDefinitionStatement FilePathParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "FilePath", null, null);

    }
}