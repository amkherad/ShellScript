using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Array;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Array
{
    public partial class BashArray : ApiArray
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashGetLength(),
            new BashCopy(),
            new BashInitialize(),
            
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}