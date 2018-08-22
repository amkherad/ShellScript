using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.MSTest.ParsingTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseFunction()
        {
            var code = "function test(int x) { echo; }";
            var parser = new Parser();

            var statements = parser.Parse(new StringReader(code), new ParserInfo(true, "", "", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }

        [TestMethod]
        public void ParseEvaluation()
        {
            var code = "var x = (46 * 34) - 23 / 4 * myFunc()";
            var parser = new Parser();

            var statements = parser.Parse(new StringReader(code), new ParserInfo(true, "", "", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }

        [TestMethod]
        public void ParseIf()
        {
            var code = new StringBuilder()
                    .AppendLine("var x = 0;")
                    .AppendLine()
                    .AppendLine("if (x == 2) {")
                    .AppendLine("    echo \"Hello\";")
                    .AppendLine("}")
                ;
            var parser = new Parser();

            var statements = parser.Parse(new StringReader(code.ToString()), new ParserInfo(true, "", "", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }
    }
}