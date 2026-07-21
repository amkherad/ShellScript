using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.IO.Path
{
    public abstract partial class ApiPath : ApiBaseClass
    {
        public const string ClassAccessName = "Path";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];
        protected static readonly FunctionParameterDefinitionStatement PathParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Path", null, null);
        protected static readonly FunctionParameterDefinitionStatement LeftParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Left", null, null);
        protected static readonly FunctionParameterDefinitionStatement RightParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Right", null, null);
    }
}
