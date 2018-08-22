namespace ShellScript.Core.Language.Sdk
{
    public interface ISdkFunc : ISdkObject
    {
        bool IsStatic { get; }
        bool AllowDynamicParams { get; }
        
        ISdkParameter[] Parameters { get; }
    }
}