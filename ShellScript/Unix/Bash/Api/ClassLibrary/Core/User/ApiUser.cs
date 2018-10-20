using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.User
{
    public partial class ApiUser : ApiBaseClass
    {
        public const string ClassAccessName = "User";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new IsSuperUser(),
            new GetUserName(),
        };
    }
}