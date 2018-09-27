namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class Exists : FileTestBase
        {
            public override string Name => "Exists";
            
            public Exists() : base("e") { }
        }
    }
}