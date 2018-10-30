using ShellScript.Core.Language.Library;

namespace ShellScript.Core.Language.Compiler
{
    public interface ILanguageObjectInfo
    {
        TypeDescriptor TypeDescriptor { get; }
        string ClassName { get; }
        string Name { get; }
        string ReName { get; }
        string AccessName { get; }
        string Fqn { get; }
    }
}