using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.CompilerServices;
using ShellScript.Core.Language.CompilerServices.Parsing;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;
using ShellScript.Core.Language.CompilerServices.Transpiling.BaseImplementations;
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
            var result = compiler.CompileFromSource(
                Console.Out,
                Console.Out,
                Console.Out,
                "/home/amk/Temp/ShellScript/variables.shellscript",
                "/home/amk/Temp/ShellScript/variables.sh",
                "unix-bash",
                CompilerFlags.CreateDefault()
            );

            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void TestCalculateEvaluation()
        {
            var parser = new Parser();

            using (var reader = new StringReader("int x = 2 + 2"))
            {
                var context = new Context(new UnixBashPlatform(), new CompilerFlags(), Console.Out, Console.Out,
                    Console.Out);
                var stt = parser.Parse(reader, new ParserInfo(Console.Out, Console.Out, true, "", "", ""));
                var definitionStt = stt.First() as DefinitionStatement;

                var result = EvaluationStatementTranspilerBase.ProcessEvaluation(context, context.GeneralScope,
                    definitionStt.DefaultValue);

                Assert.IsNotNull(result);
            }
        }
    }
}