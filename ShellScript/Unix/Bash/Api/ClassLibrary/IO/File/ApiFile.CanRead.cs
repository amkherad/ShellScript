namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class CanRead : FileTestBase
        {
            public override string Name => "CanRead";
            
            public CanRead() : base("r") { }
        }
    }
}