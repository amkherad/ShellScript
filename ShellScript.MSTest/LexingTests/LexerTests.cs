using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.CompilerServices.Lexing;

namespace ShellScript.MSTest.LexingTests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void TestLexing()
        {
            var lexer = new Lexer();

            var syntax = new StringBuilder()
                .AppendLine("var x = 23;");

            var tokens = lexer.Tokenize(new StringReader(syntax.ToString()));

            foreach (var token in tokens)
            {
                Trace.WriteLine($"{token.Type}: ({token.Value})");
            }
        }
    }
}