using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Core.String
{
    public partial class ApiString
    {
        public class IsNullOrEmpty : TestCommandBase
        {
            public override string Name => nameof(IsNullOrEmpty);
            public override string Summary { get; }
            public override string ClassName => IO.File.ApiFile.ClassAccessName;
            
            public IsNullOrEmpty() : base("z") { }
        }
    }
}