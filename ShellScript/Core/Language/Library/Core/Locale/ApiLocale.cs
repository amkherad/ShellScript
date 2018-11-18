namespace ShellScript.Core.Language.Library.Core.Locale
{
    public abstract partial class ApiLocale : ApiBaseClass
    {
        public const string ClassAccessName = "Locale";
        public override string Name => ClassAccessName;

        public override IApiVariable[] Variables => new IApiVariable[0];
    }
}