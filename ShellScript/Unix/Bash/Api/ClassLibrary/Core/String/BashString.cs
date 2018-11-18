using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.String;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class BashString : ApiString
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashGetLength(),
            
            new BashIsNullOrEmpty(),
            new BashIsNullOrWhiteSpace(),
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}