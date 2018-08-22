using System.Linq;

namespace ShellScript.Core.Language.Sdk
{
    public abstract class SdkBaseClass : ISdkClass
    {
        public abstract string Name { get; }
        
        public abstract ISdkVariable[] Variables { get; }
        public abstract ISdkFunc[] Functions { get; }

        
        public virtual bool TryGetFunction(string functionName, out ISdkFunc result)
        {
            result = Functions.FirstOrDefault(x => x.Name == functionName);
            return result != null;
        }

        public virtual bool TryGetVariable(string variableName, out ISdkVariable result)
        {
            result = Variables.FirstOrDefault(x => x.Name == variableName);
            return result != null;
        }
    }
}