namespace ShellScript.Core.Language.Library.Core.User
{
    public abstract partial class ApiUser : ApiBaseClass
    {
        public const string ClassAccessName = "User";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}