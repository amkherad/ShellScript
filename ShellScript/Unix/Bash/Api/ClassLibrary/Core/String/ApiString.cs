using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class ApiString : ApiBaseClass
    {
        public const string ClassAccessName = "String";
        public override string Name => ClassAccessName;

        public static readonly FunctionParameterDefinitionStatement StringParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.String, "String", null, null);
        
        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new GetLength(),
            
            new IsNullOrEmpty(),
            new IsNullOrWhiteSpace(),
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}