using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;
using ShellScript.Windows.Batch;
using ShellScript.Windows.PowerShell;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class WindowsPlatformCompilationTests
    {
        [TestInitialize]
        public void Init()
        {
            Platforms.AddPlatform(new UnixBashPlatform());
            Platforms.AddPlatform(new WindowsPowerShellPlatform());
            Platforms.AddPlatform(new WindowsBatchPlatform());
        }

        [TestMethod]
        public void PowerShell_EchoExample_CompilesWithWriteOutput()
        {
            var output = CompilePlatform("Windows-PowerShell", ".ps1",
                "echo \"plain\";",
                "string name = \"x\";",
                "echo name;");

            StringAssert.Contains(output, "Write-Output");
            StringAssert.Contains(output, "$name = ");
        }

        [TestMethod]
        public void Batch_EchoExample_CompilesWithSetAndEcho()
        {
            var output = CompilePlatform("Windows-Batch", ".cmd",
                "echo \"plain\";",
                "string name = \"x\";",
                "echo name;");

            StringAssert.Contains(output, "@echo off");
            StringAssert.Contains(output, "set \"name=");
            StringAssert.Contains(output, "echo ");
        }

        private static string CompilePlatform(string platformName, string extension, params string[] lines)
        {
            var path = Path.Combine(Path.GetTempPath(), "win-" + Guid.NewGuid().ToString("N") + ".shellscript");
            File.WriteAllLines(path, lines);
            var output = Path.Combine(Path.GetTempPath(), "win-" + Guid.NewGuid().ToString("N") + extension);
            var compiler = new Compiler();
            using (var errors = new StringWriter())
            using (var warnings = new StringWriter())
            using (var logs = new StringWriter())
            {
                var result = compiler.CompileFromSource(errors, warnings, logs, path, output, platformName,
                    CompilerFlags.CreateDefault());
                Assert.IsTrue(result.Successful, errors.ToString());
                return File.ReadAllText(output);
            }
        }
    }
}
