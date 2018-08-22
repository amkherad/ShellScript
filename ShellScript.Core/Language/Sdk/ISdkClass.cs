namespace ShellScript.Core.Language.Sdk
{
    public interface ISdkClass : ISdkObject
    {
        ISdkVariable[] Variables { get; }
        
        ISdkFunc[] Functions { get; }
        
        
        bool TryGetFunction(string functionName, out ISdkFunc result);
        
        bool TryGetVariable(string variableName, out ISdkVariable result);
    }
}