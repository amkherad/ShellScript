using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Convert;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Convert
{
    public partial class BashConvert : ApiConvert
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashToInteger(),
            new BashToFloat(),
            new BashToNumber(),
            new BashToBoolean(),
            
            new BashToString(),
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}