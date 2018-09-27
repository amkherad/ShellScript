namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class CanWrite : FileTestBase
        {
            public override string Name => "CanWrite";
            
            public CanWrite() : base("r") { }
        }
    }
}