using ShellScript.Unix.Bash.Api.ClassLibrary.Base;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile
    {
        public abstract class FileTestCommandBase : TestCommandBase
        {
            public override string ClassName => ClassAccessName;
            
            public FileTestCommandBase(string testCharacter) : base(testCharacter)
            {
            }
        }
    }
}