using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class BashConditionalExpressionTests
    {
        [TestMethod]
        public void CompilesBooleanAndStartsWithAsValidBashIf()
        {
            var output = Compile(
                "#!/usr/bin/env shellscript",
                "bool flag = true;",
                "string key = \"MOUSE:1:2:0\";",
                "if (flag && String.StartsWith(key, \"MOUSE:\")) {",
                "  Console.WriteLine(\"ok\");",
                "}");

            StringAssert.Contains(output, "if [ $flag -ne 0 ] && [[ \"$key\" == \"MOUSE:\"* ]]");
            Assert.IsFalse(output.Contains("[ [ $flag -ne 0 ] -ne 0 ]"), output);
        }

        [TestMethod]
        public void CompilesNotIsNullOrEmptyAsValidBashTest()
        {
            var output = Compile(
                "#!/usr/bin/env shellscript",
                "string key = \"x\";",
                "if (!String.IsNullOrEmpty(key)) {",
                "  Console.WriteLine(\"ok\");",
                "}");

            StringAssert.Contains(output, "if [ -n \"$key\" ]");
            Assert.IsFalse(output.Contains("[ ! [ -z"), output);
        }

        [TestMethod]
        public void CompilesNumericComparisonsCombinedWithLogicalAnd()
        {
            var output = Compile(
                "#!/usr/bin/env shellscript",
                "int rowIndex = 0;",
                "int scroll = 0;",
                "int drawn = 0;",
                "int bodyRows = 10;",
                "if (rowIndex >= scroll && drawn < bodyRows) {",
                "  Console.WriteLine(\"ok\");",
                "}");

            StringAssert.Contains(output, "if [ $rowIndex -ge $scroll ] && [ $drawn -lt $bodyRows ]");
            Assert.IsFalse(output.Contains("-ge $scroll -ne 0"), output);
        }

        private static string Compile(params string[] lines)
        {
            Platforms.AddPlatform(new UnixBashPlatform());
            var path = Path.Combine(Path.GetTempPath(), "cond-" + Guid.NewGuid().ToString("N") + ".shellscript");
            File.WriteAllLines(path, lines);
            var output = Path.Combine(Path.GetTempPath(), "cond-" + Guid.NewGuid().ToString("N") + ".bash");
            var compiler = new Compiler();
            using (var errors = new StringWriter())
            using (var warnings = new StringWriter())
            using (var logs = new StringWriter())
            {
                var result = compiler.CompileFromSource(errors, warnings, logs, path, output, "Unix-Bash",
                    CompilerFlags.CreateDefault());
                Assert.IsTrue(result.Successful, errors.ToString() + warnings.ToString());
                return File.ReadAllText(output);
            }
        }
    }
}
