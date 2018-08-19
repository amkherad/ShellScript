using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.Compiler.Parsing;

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

            var statements = parser.Parse(new StringReader(code), new ParserInfo("", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }
    }
}