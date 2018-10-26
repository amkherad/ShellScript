using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Network.Net
{
    public partial class ApiNet : ApiBaseClass
    {
        public const string ClassAccessName = "Net";
        public override string Name => "Net";

        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new Ping(),
        };
    }
}