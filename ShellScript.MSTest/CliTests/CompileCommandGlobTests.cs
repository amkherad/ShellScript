using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.CommandLine;
using ShellScript.Core.Language;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CliTests
{
    [TestClass]
    public class CompileCommandGlobTests
    {
        [TestInitialize]
        public void Init()
        {
            if (Platforms.GetPlatformByName("Unix-Bash") == null)
            {
                Platforms.AddPlatform(new UnixBashPlatform());
            }
        }

        [TestMethod]
        public void GetOutputPathBesideSource_UsesPlatformExtension()
        {
            var platform = new UnixBashPlatform();
            var path = CompileCommand.GetOutputPathBesideSource("/tmp/Foo/Foo.shellscript", platform);
            Assert.AreEqual(Path.Combine("/tmp", "Foo", "Foo.bash"), path);
        }

        [TestMethod]
        public void CompileGlob_WritesOutputBesideEachSource()
        {
            var temp = Path.Combine(Path.GetTempPath(), "ss-compile-glob-" + Guid.NewGuid().ToString("N"));
            var sub = Path.Combine(temp, "Sub");
            Directory.CreateDirectory(sub);

            var script = Path.Combine(sub, "Demo.shellscript");
            File.WriteAllText(script, "echo \"ok\";");

            var expectedOut = Path.Combine(sub, "Demo.bash");
            try
            {
                using (var outWriter = new StringWriter())
                using (var errWriter = new StringWriter())
                {
                    var ctx = CommandContext.Parse(new[]
                    {
                        "compile",
                        Path.Combine(sub, "*.shellscript"),
                        "Unix-Bash",
                    });

                    var cmd = new CompileCommand();
                    var code = cmd.Execute(outWriter, errWriter, errWriter, errWriter, ctx);
                    Assert.AreEqual(ResultCodes.Successful, code);
                }

                Assert.IsTrue(File.Exists(expectedOut), "Expected compiled output beside source.");
                StringAssert.Contains(File.ReadAllText(expectedOut), "echo");
            }
            finally
            {
                try
                {
                    Directory.Delete(temp, true);
                }
                catch
                {
                    // Best-effort cleanup.
                }
            }
        }
    }
}
