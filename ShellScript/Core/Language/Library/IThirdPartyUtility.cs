using System.IO;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Core.Language.Library
{
    public interface IThirdPartyUtility
    {
        string Name { get; }

        string WriteExistenceCondition(Context context, TextWriter nonInlinePartWriter);
    }
}