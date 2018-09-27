using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.IO.File
{
    public partial class ApiFile : ApiBaseClass
    {
        public const string ClassAccessName = "File";
        public override string Name => "File";

        public override IApiVariable[] Variables => new IApiVariable[0];

        public override IApiFunc[] Functions { get; } =
        {
            new Exists(),
            new CanRead(),
            new CanWrite(),
            new CanExecute(),
            new IsLink(),
        };
    }
}