using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Environment
{
    public abstract partial class ApiEnvironment : ApiBaseClass
    {
        public const string ClassAccessName = "Environment";
        public override string Name => ClassAccessName;
        public override IApiVariable[] Variables => new IApiVariable[0];
        protected static readonly FunctionParameterDefinitionStatement NameParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Name", null, null);
    }
}
