namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class CanWrite : FileTestBase
        {
            public override string Name => "CanWrite";
            public override string Summary => "Checks whether a file has write permission.";
            
            public CanWrite() : base("r") { }
        }
    }
}