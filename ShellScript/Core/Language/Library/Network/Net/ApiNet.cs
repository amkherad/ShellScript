namespace ShellScript.Core.Language.Library.Network.Net
{
    public abstract partial class ApiNet : ApiBaseClass
    {
        public const string ClassAccessName = "Net";
        public override string Name => "Net";

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}