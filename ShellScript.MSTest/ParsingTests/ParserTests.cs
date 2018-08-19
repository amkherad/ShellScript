using System;
using System.IO;
using System.Linq;
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

            var statements = parser.Parse(new StringReader(code), new ParserInfo(true, "", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }
    }
}