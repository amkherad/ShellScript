using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        
        [TestMethod]
        public void TestTokens()
        {
            var lexer = new Lexer();

            var syntax =
                "\"ali\" 'ali' ( /* x*/ ) {/*x */ } [  /*x " +
                Environment.NewLine +
                "*/] && & || | . , = == != ! ~ ++ -- * / + - \\ if else #if #elseif #else #endif for foreach do while loop class function throw async await in notin like notlike call const var int double float object variant number decimal int[] double[] float[] object[] variant[] number[] decimal[] echo 343 +324.53 +34e-23";

            var tokens = lexer.Tokenize(new StringReader(syntax)).ToList();

            GC.KeepAlive(tokens);
        }
        
        [TestMethod]
        public void TestSingleLineComment()
        {
            var code = "var x = (46 * 34) - 23 / 4 // * myFunc() //Ali Mousavi Kherad";
            
            var lexer = new Lexer();

            var statements = lexer.Tokenize(new StringReader(code)).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }
    }
}