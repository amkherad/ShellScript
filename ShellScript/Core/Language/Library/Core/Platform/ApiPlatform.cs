namespace ShellScript.Core.Language.Library.Core.Platform
{
    public abstract partial class ApiPlatform : ApiBaseClass
    {
        public const string ClassAccessName = "Platform";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}