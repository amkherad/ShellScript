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
            new BashContains(),
            new BashStartsWith(),
            new BashEndsWith(),
            new BashToLower(),
            new BashToUpper(),
            new BashTrim(),
            new BashGetBefore(),
            new BashGetAfter(),
            new BashIndexOf(),
            new BashSubstring(),
            new BashEquals(),
            new BashReplace(),
            new BashJoin(),
            new BashSplit(),
            new BashRepeat(),
            new BashTrimStart(),
            new BashTrimEnd(),
            new BashPadLeft(),
            new BashPadRight(),
            new BashLastIndexOf(),
            new BashCompare(),
            new BashCompareIgnoreCase(),
            new BashContainsIgnoreCase(),
        };
    }
}