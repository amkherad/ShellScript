using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Library;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class ConstantFoldingTests
    {
        [TestMethod]
        public void Compile_FoldsLiteralEchoExpressions()
        {
            var script = WriteTempScript(
                "echo 3 + 4;",
                "echo true && false;",
                "echo !(1 == 1);");

            var bash = CompileToString(script);

            StringAssert.Contains(bash, "echo \"7\"");
            StringAssert.Contains(bash, "echo \"0\"");
        }

        [TestMethod]
        public void TryFoldCast_TruncatesFloatingLiteralToInteger()
        {
            var info = new StatementInfo("test", 1, 1);
            var source = new ConstantValueStatement(TypeDescriptor.Float, "42.9", info);
            var folded = ConstantFolding.TryFoldCast(TypeDescriptor.Integer, source, info);

            Assert.IsNotNull(folded);
            Assert.AreEqual("42", folded.Value);
        }

        [TestMethod]
        public void UseConstantFoldingFlag_DisablesLiteralFolding()
        {
            var script = WriteTempScript("echo 2 * 3;");
            var flags = CompilerFlags.CreateDefault();
            flags.UseConstantFolding = false;

            var bash = CompileToString(script, flags);

            Assert.IsFalse(bash.Contains("echo 6"), "literal multiplication should not fold when disabled");
            StringAssert.Contains(bash, "2 * 3");
        }

        private static string WriteTempScript(params string[] lines)
        {
            var path = Path.Combine(Path.GetTempPath(), "fold-" + Guid.NewGuid().ToString("N") + ".shellscript");
            File.WriteAllLines(path, new[] {"#!/usr/bin/env shellscript"}.Concat(lines));
            return path;
        }

        private static string CompileToString(string scriptPath, CompilerFlags flags = null)
        {
            Platforms.AddPlatform(new UnixBashPlatform());
            flags ??= CompilerFlags.CreateDefault();
            var output = Path.Combine(Path.GetTempPath(), "fold-" + Guid.NewGuid().ToString("N") + ".bash");
            var compiler = new Compiler();
            var platform = new UnixBashPlatform();
            flags = platform.ReviseFlags(flags);

            using (var errors = new StringWriter())
            using (var warnings = new StringWriter())
            using (var logs = new StringWriter())
            {
                var result = compiler.CompileFromSource(errors, warnings, logs, scriptPath, output,
                    "Unix-Bash", flags);
                Assert.IsTrue(result.Successful, errors.ToString());
            }

            return File.ReadAllText(output);
        }
    }
}
