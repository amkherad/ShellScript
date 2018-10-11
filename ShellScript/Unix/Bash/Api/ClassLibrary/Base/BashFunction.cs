using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Bash.Api.ClassLibrary.Base
{
    public abstract class BashFunction : ApiBaseFunction
    {
        public const string AwkUtilityName = "awk";
        public const string BcUtilityName = "bc";
        
        public override bool IsStatic => true;
        public override bool AllowDynamicParams => false;
    }
}