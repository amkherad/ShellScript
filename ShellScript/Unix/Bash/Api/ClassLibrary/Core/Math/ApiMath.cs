using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class ApiMath : ApiBaseClass
    {
        public const string ClassAccessName = "Math";
        public override string Name => ClassAccessName;

        public static readonly FunctionParameterDefinitionStatement NumberParameter =
            new FunctionParameterDefinitionStatement(TypeDescriptor.Numeric, "Number", null, null);
        
        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new Abs(),
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}