namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class CanRead : FileTestCommandBase
        {
            public override string Name => "CanRead";
            public override string Summary => "Checks whether a file has read permission.";
            
            public CanRead() : base("r") { }
        }
    }
}