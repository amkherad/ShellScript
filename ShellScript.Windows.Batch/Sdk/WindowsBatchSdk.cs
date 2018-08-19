using ShellScript.Core.Language.Sdk;

namespace ShellScript.Windows.Batch.Sdk
{
    public class WindowsBatchSdk : ISdk
    {
        public string Name => "Batch";
        public string OutputFileExtension => "bat";


        public bool TryGetClass(string className, out ISdkClass result)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetGeneralFunction(string functionName, out ISdkFunc result)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetGeneralVariable(string variableName, out ISdkVariable result)
        {
            throw new System.NotImplementedException();
        }
    }
}