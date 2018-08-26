using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class CompilerTests
    {
        [TestMethod]
        public void TestCompiler()
        {
            Platforms.AddPlatform(new UnixBashPlatform());

            var compiler = new Compiler();
            var result = compiler.CompileFromSource("/home/amk/Temp/ShellScript/variables.shellscript", "/home/amk/Temp/ShellScript/variables.sh", "unix-bash", true);
            
            Assert.IsTrue(result.Successful);
        }
    }
}