using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Array
{
    public abstract partial class ApiArray : ApiBaseClass
    {
        public const string ClassAccessName = "Array";
        public override string Name => ClassAccessName;

        public static readonly FunctionParameterDefinitionStatement ArrayParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.Void, "Array", null, null, true);
        
        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}