namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class IsLink : FileTestBase
        {
            public override string Name => "IsLink";
            public override string Summary => "Checks whether a file exists and is a link to another file.";
            
            public IsLink() : base("L") { }
        }
    }
}