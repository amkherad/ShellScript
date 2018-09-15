using ShellScript.Core.Language.CompilerServices.Transpiling;

namespace ShellScript.Core.Language.Library
{
    public interface IApi
    {
        IApiVariable[] Variables { get; }
        IApiFunc[] Functions { get; }
        IApiClass[] Classes { get; }
        
        string Name { get; }
        

        void InitializeContext(Context context);
        
        
        bool TryGetClass(string className, out IApiClass result);

        bool TryGetGeneralFunction(string functionName, out IApiFunc result);

        bool TryGetGeneralVariable(string variableName, out IApiVariable result);
    }
}