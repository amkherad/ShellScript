using System.IO;
using ShellScript.Core.Language.Compiler.Transpiling;

namespace ShellScript.Unix.Utilities
{
    public class PythonThirdPartyUtility : BashBasicThirdPartyUtility
    {
        public override string Name => "python";

        public override string WriteExistenceCondition(Context context, TextWriter nonInlinePartWriter)
        {
            nonInlinePartWriter.WriteLine("command -v python3 > /dev/null");
            return "$? -eq 0";
        }
    }
}