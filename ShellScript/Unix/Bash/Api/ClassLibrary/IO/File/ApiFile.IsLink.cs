namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class IsLink : FileTestBase
        {
            public override string Name => "IsLink";
            
            public IsLink() : base("L") { }
        }
    }
}