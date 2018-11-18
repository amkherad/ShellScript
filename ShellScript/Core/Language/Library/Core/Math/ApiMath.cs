using ShellScript.Core.Language.Compiler.Statements;

namespace ShellScript.Core.Language.Library.Core.Math
{
    public abstract partial class ApiMath : ApiBaseClass
    {
        public const string ClassAccessName = "Math";
        public override string Name => ClassAccessName;

        public static readonly FunctionParameterDefinitionStatement NumberParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.Numeric, "Number", null, null);
        
        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}