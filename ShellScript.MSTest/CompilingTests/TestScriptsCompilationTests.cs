using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.Compiler;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class TestScriptsCompilationTests
    {
        [TestMethod]
        public void TestScripts()
        {
            const string TestScriptsRootDirectoryName = "ShellScript.MSTest";
            const string TestScriptsDirectoryName = "TestScripts";
            const string TempOutputDirectoryName = "TestScripts.temp";

            var platforms = new[]
            {
                new UnixBashPlatform()
            };
            var compilerFlags = CompilerFlags.CreateDefault();
            //compilerFlags.WriteShellScriptVersion = false;

            string testScriptsRoot = null;
            string tempOutputRoot = null;
            
            var cDir = Environment.CurrentDirectory;
            while (!string.IsNullOrWhiteSpace(cDir))
            {
                if (StringComparer.InvariantCultureIgnoreCase.Equals(Path.GetFileName(cDir),
                    TestScriptsRootDirectoryName))
                {
                    testScriptsRoot = Path.Combine(cDir, TestScriptsDirectoryName);
                    tempOutputRoot = Path.Combine(cDir, TempOutputDirectoryName);
                    break;
                }

                cDir = Path.GetDirectoryName(cDir);
            }

            if (testScriptsRoot == null)
            {
                Assert.Fail("Test scripts directory not found.");
            }

            foreach (var file in Directory.GetFiles(testScriptsRoot, "*.shellscript", SearchOption.AllDirectories))
            {
                foreach (var platform in platforms)
                {
                    var ext = platform.Name.ToLower();
                    var assertionFile = $"{Path.GetFileNameWithoutExtension(file)}.{ext}";
                    var outputFile = $"{Path.GetFileNameWithoutExtension(file)}.{ext}";

                    assertionFile = Path.Combine(testScriptsRoot, assertionFile);
                    outputFile = Path.Combine(tempOutputRoot, outputFile);
                    
                    if (File.Exists(assertionFile))
                    {
                        using (var logWriter = new StringWriter())
                        {
                            Directory.CreateDirectory(tempOutputRoot);

                            var compiler = new Compiler();
                            compiler.CompileFromSource(file, tempOutputRoot, outputFile,
                                platform, compilerFlags, logWriter, logWriter, logWriter);

                            using (var assertionFileStream =
                                new FileStream(assertionFile, FileMode.Open, FileAccess.Read))
                            using (var outputFileStream = new FileStream(outputFile, FileMode.Open, FileAccess.Read))
                            using (var assertionReader = new StreamReader(assertionFileStream))
                            using (var outputReader = new StreamReader(outputFileStream))
                            {
                                string assertLine;
                                string outputLine;

                                int line = 0;
                                
                                while ((assertLine = assertionReader.ReadLine()) != null &&
                                       (outputLine = outputReader.ReadLine()) != null)
                                {
                                    if (line++ >= 2)
                                    {
                                        if (assertLine != outputLine)
                                        {
                                            Assert.Fail($"Assert failed on {file}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}