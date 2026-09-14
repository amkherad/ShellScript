using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Directory
{
    public abstract partial class ApiDirectory : ApiBaseClass
    {
        public const string ClassAccessName = "Directory";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];

        public static readonly FunctionParameterDefinitionStatement DirectoryPathParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "DirectoryPath", null, null);
    }
}
