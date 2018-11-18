using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Math;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class BashMath : ApiMath
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashAbs(),
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}