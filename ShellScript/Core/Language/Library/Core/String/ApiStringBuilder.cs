using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public abstract partial class ApiStringBuilder : ApiBaseClass
    {
        public const string ClassAccessName = "StringBuilder";
        public override string Name => ClassAccessName;

        public static readonly FunctionParameterDefinitionStatement BuilderParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "Builder", null, null);

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}
