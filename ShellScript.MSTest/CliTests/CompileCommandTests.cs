using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.CommandLine;
using ShellScript.Core;
using ShellScript.Core.Language;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CliTests
{
    [TestClass]
    public class CompileCommandTests
    {
        [TestMethod]
        public void TestCompile()
        {
            var compileCommand = new CompileCommand();

            var outputWriter = Console.Out; //new StreamWriter(Console.OpenStandardError());
            var errorWriter = new ColoredWriter(Console.Out, ConsoleColor.Red); //new StreamWriter(Console.OpenStandardError());

            var commandContext = CommandContext.Parse(new[]
            {
                "/home/amk/Temp/ShellScript/variables.shellscript",
                "/home/amk/Temp/ShellScript/variables.sh",
                "unix-bash",
                "--verbose",
                "--use-explicit-echo-dev",
            });

            Platforms.AddPlatform(new UnixBashPlatform());

            compileCommand.Execute(outputWriter, errorWriter, outputWriter, outputWriter, commandContext);
        }
    }
}