using ShellScript.Core.Language.Library;
using ShellScript.Core.Language.Library.Core.User;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.User
{
    public partial class BashUser : ApiUser
    {
        public override IApiFunc[] Functions { get; } =
        {
            new BashIsSuperUser(),
            new BashGetUserName(),
        };
    }
}