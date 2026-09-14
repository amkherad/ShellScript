using System.IO;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Utilities
{
    public class JqThirdPartyUtility : BashBasicThirdPartyUtility
    {
        public override string Name => "jq";

        public override string WriteExistenceCondition(Context context, TextWriter nonInlinePartWriter)
        {
            nonInlinePartWriter.WriteLine("command -v jq > /dev/null");
            return "$? -eq 0";
        }
    }
}
