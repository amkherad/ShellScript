namespace ShellScript.Core.Language.Sdk
{
    public abstract class SdkBaseFunction : ISdkFunc
    {
        public abstract string Name { get; }
        public abstract bool IsStatic { get; }
        public abstract bool AllowDynamicParams { get; }
        public abstract ISdkParameter[] Parameters { get; }
    }
}