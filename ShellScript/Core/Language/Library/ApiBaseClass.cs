using System.Linq;

namespace ShellScript.Core.Language.Library
{
    public abstract class ApiBaseClass : IApiClass
    {
        public abstract string Name { get; }
        
        public abstract IApiVariable[] Variables { get; }
        public abstract IApiFunc[] Functions { get; }

        
        public virtual bool TryGetFunction(string functionName, out IApiFunc result)
        {
            result = Functions.FirstOrDefault(x => x.Name == functionName);
            return result != null;
        }

        public virtual bool TryGetVariable(string variableName, out IApiVariable result)
        {
            result = Variables.FirstOrDefault(x => x.Name == variableName);
            return result != null;
        }
    }
}