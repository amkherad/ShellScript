using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.Platform
{
    public partial class ApiPlatform : ApiBaseClass
    {
        public const string ClassAccessName = "Platform";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new Call(),
            new CallDecimal(),
        };
    }
}