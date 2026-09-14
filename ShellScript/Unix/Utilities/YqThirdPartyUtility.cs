using System.IO;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Utilities
{
    public class YqThirdPartyUtility : BashBasicThirdPartyUtility
    {
        public override string Name => "yq";

        public override string WriteExistenceCondition(Context context, TextWriter nonInlinePartWriter)
        {
            nonInlinePartWriter.WriteLine("command -v yq > /dev/null");
            return "$? -eq 0";
        }
    }
}
