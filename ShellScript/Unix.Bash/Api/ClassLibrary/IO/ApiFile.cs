using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO
{
    public partial class ApiFile : ApiBaseClass
    {
        public override string Name => "File";

        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new Exists(),
        };
    }
}