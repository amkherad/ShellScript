namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class CanExecute : FileTestBase
        {
            public override string Name => "CanExecute";
            public override string Summary => "Checks whether a file has execute permission.";
            
            public CanExecute() : base("x") { }
        }
    }
}