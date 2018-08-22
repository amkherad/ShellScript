using System.Linq;

namespace ShellScript.Core.Language.Sdk
{
    public abstract class SdkBase : ISdk
    {
        public abstract ISdkVariable[] Variables { get; }
        public abstract ISdkFunc[] Functions { get; }
        public abstract ISdkClass[] Classes { get; }
        
        public abstract string Name { get; }
        public abstract string OutputFileExtension { get; }
        

        public bool TryGetClass(string className, out ISdkClass result)
        {
            result = Classes.FirstOrDefault(x => x.Name == className);
            return result != null;
        }

        public bool TryGetGeneralFunction(string functionName, out ISdkFunc result)
        {
            result = Functions.FirstOrDefault(x => x.Name == functionName);
            return result != null;
        }

        public bool TryGetGeneralVariable(string variableName, out ISdkVariable result)
        {
            result = Variables.FirstOrDefault(x => x.Name == variableName);
            return result != null;
        }
    }
}