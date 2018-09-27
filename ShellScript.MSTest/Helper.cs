using System;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Parsing;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest
{
    public class Helper
    {
        public static Context CreateBashContext()
        {
            return new Context(
                new UnixBashPlatform(),
                new CompilerFlags(),
                Console.Out,
                Console.Out,
                Console.Out);
        }

        public static ParserInfo CreateParserInfo()
        {
            return new ParserInfo(Console.Out, Console.Out, true, "", "", "");
        }
    }
}