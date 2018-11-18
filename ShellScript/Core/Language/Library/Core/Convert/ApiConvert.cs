namespace ShellScript.Core.Language.Library.Core.Convert
{
    public abstract partial class ApiConvert : ApiBaseClass
    {
        public const string ClassAccessName = "Convert";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}