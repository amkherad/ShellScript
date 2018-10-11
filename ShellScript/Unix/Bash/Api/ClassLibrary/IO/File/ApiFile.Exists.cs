namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class Exists : FileTestCommandBase
        {
            public override string Name => "Exists";
            public override string Summary => "Checks whether a file exists.";
            
            public Exists() : base("e") { }
        }
    }
}