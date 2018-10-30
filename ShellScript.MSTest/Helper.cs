using System;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Parsing;
using ShellScript.Core.Language.Compiler.Transpiling;
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

        public static ParserContext CreateParserInfo()
        {
            return new ParserContext(Console.Out, Console.Out, true, "", "", "");
        }
    }
}