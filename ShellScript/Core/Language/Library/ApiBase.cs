using System.Linq;

namespace ShellScript.Core.Language.Library
{
    public abstract class ApiBase : IApi
    {
        public abstract IApiVariable[] Variables { get; }
        public abstract IApiFunc[] Functions { get; }
        public abstract IApiClass[] Classes { get; }
        
        public abstract string Name { get; }
        public abstract string OutputFileExtension { get; }
        

        public bool TryGetClass(string className, out IApiClass result)
        {
            result = Classes.FirstOrDefault(x => x.Name == className);
            return result != null;
        }

        public bool TryGetGeneralFunction(string functionName, out IApiFunc result)
        {
            result = Functions.FirstOrDefault(x => x.Name == functionName);
            return result != null;
        }

        public bool TryGetGeneralVariable(string variableName, out IApiVariable result)
        {
            result = Variables.FirstOrDefault(x => x.Name == variableName);
            return result != null;
        }
    }
}