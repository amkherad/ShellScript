using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.String
{
    public abstract partial class ApiString : ApiBaseClass
    {
        public const string ClassAccessName = "String";
        public override string Name => ClassAccessName;

        public static readonly FunctionParameterDefinitionStatement StringParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "String", null, null);
        
        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}