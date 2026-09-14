using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.Math;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class BashMath : ApiMath
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashAbs(),
            new BashMin(),
            new BashMax(),
            new BashFloor(),
            new BashCeiling(),
            new BashRound(),
            new BashTruncate(),
            new BashSqrt(),
            new BashPow(),
            new BashSign(),
        };
    }
}