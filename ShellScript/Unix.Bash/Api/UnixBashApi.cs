using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash.Api.ClassLibrary.IO;

namespace ShellScript.Unix.Bash.Api
{
    public class UnixBashApi : ApiBase
    {
        public override IApiVariable[] Variables => new IApiVariable[0];
        public override IApiFunc[] Functions => new IApiFunc[0];
        public override IApiClass[] Classes { get; } =
        {
            new ApiFile()
        };
        
        public override string Name => "Unix-Bash";
        public override string OutputFileExtension => "sh";
    }
}