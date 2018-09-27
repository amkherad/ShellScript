namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public class CanExecute : FileTestBase
        {
            public override string Name => "CanExecute";
            
            public CanExecute() : base("x") { }
        }
    }
}