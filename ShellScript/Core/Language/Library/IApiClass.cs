namespace ShellScript.Core.Language.Library
{
    public interface IApiClass : IApiObject
    {
        IApiVariable[] Variables { get; }
        
        IApiFunc[] Functions { get; }
        
        
        bool TryGetFunction(string functionName, out IApiFunc result);
        
        bool TryGetVariable(string variableName, out IApiVariable result);
    }
}