using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Math
{
    public partial class ApiMath : ApiBaseClass
    {
        public const string ClassAccessName = "Math";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new Abs(),
        };
    }
}