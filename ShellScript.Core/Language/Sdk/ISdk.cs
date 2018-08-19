namespace ShellScript.Core.Language.Sdk
{
    public interface ISdk
    {
        string Name { get; }
        
        string OutputFileExtension { get; }
        
        
        bool TryGetClass(string className, out ISdkClass result);

        bool TryGetGeneralFunction(string functionName, out ISdkFunc result);

        bool TryGetGeneralVariable(string variableName, out ISdkVariable result);
    }
}