using System.IO;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Library;

namespace ShellScript.Unix.Utilities
{
    public abstract class BashBasicThirdPartyUtility : IThirdPartyUtility
    {
        public abstract string Name { get; }

        public string WriteExistenceCondition(Context context, TextWriter nonInlinePartWriter)
        {
            nonInlinePartWriter.WriteLine($"command -v {Name} > /dev/null");
            return "$? -eq 0";
        }
    }
}