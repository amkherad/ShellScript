using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Locale;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Locale
{
    public partial class BashLocale : ApiLocale
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashGetCurrentLocale(),
            //new ShellScriptResourceFunction(ClassAccessName, "Truncate", "ApiMath_Truncate.shellscript", null,
            //    DataTypes.Numeric, false, true, new [] { NumberParameter }
            //),
        };
    }
}