using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Unix.Bash.PlatformTranspiler.ExpressionBuilders;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class BashStringCallParameterTests
    {
        [TestMethod]
        public void NormalizeStringConcatForAssignment_removes_stray_quotes()
        {
            var input = "\"${CBold}| ${host} | \"${total}\" sockets\"";
            var normalized = BashDefaultExpressionBuilder.NormalizeStringConcatForAssignment(input);
            Assert.AreEqual("${CBold}| ${host} | ${total} sockets", normalized);
        }
    }
}
