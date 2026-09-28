using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class OptimizationTests
    {
        [TestMethod]
        public void DeadBranchElimination_RemovesWhileFalseBody()
        {
            var bash = Compile(
                "while (false) {",
                "    echo \"removed\";",
                "}");

            Assert.IsFalse(bash.Contains("removed"));
            Assert.IsFalse(bash.Contains("while"));
        }

        [TestMethod]
        public void DeadBranchElimination_FoldsIfTrue()
        {
            var bash = Compile(
                "if (true) {",
                "    echo \"kept\";",
                "} else {",
                "    echo \"dropped\";",
                "}");

            StringAssert.Contains(bash, "kept");
            Assert.IsFalse(bash.Contains("dropped"));
            Assert.IsFalse(bash.Contains("\nif "));
        }

        private static string Compile(params string[] lines)
        {
            Platforms.AddPlatform(new UnixBashPlatform());
            var path = Path.Combine(Path.GetTempPath(), "opt-" + Guid.NewGuid().ToString("N") + ".shellscript");
            File.WriteAllLines(path, new[] {"#!/usr/bin/env shellscript"}.Concat(lines));
            var output = Path.Combine(Path.GetTempPath(), "opt-" + Guid.NewGuid().ToString("N") + ".bash");
            var compiler = new Compiler();
            using (var errors = new StringWriter())
            using (var warnings = new StringWriter())
            using (var logs = new StringWriter())
            {
                var result = compiler.CompileFromSource(errors, warnings, logs, path, output, "Unix-Bash",
                    CompilerFlags.CreateDefault());
                Assert.IsTrue(result.Successful, errors.ToString());
            }

            return File.ReadAllText(output);
        }
    }
}
