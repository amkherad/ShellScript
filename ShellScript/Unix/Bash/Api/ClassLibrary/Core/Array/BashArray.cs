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
            new BashIndexOf(),
            new BashContains(),
            new BashClear(),
            new BashReverse(),
            new BashAdd(),
        };
    }
}