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

            GC.KeepAlive(result);
        }

        [TestMethod]
        public void TestDecimalCalculationEvaluation()
        {
            using (var reader = new StringReader("int x = 2 + 2"))
            using (var metaWriter = new StringWriter())
            using (var codeWriter = new StringWriter())
            {
                var context = Helper.CreateBashContext();
                var parser = new Parser(context);

                var stt = parser.Parse(reader, Helper.CreateParserInfo());
                var definitionStt = (DefinitionStatement) stt.First();

                var transpiler = context.GetEvaluationTranspilerForStatement(definitionStt.DefaultValue);
                var (dataType, exp, template) = transpiler.GetExpression(context, context.GeneralScope, metaWriter,
                    codeWriter, definitionStt, definitionStt.DefaultValue);

                Assert.AreEqual("4", exp);
            }
        }

        [TestMethod]
        public void TestBooleanCalculationEvaluation()
        {
            using (var reader = new StringReader("int x = true || false || true && false"))
            using (var metaWriter = new StringWriter())
            using (var codeWriter = new StringWriter())
            {
                var context = Helper.CreateBashContext();
                var parser = new Parser(context);

                var stt = parser.Parse(reader, Helper.CreateParserInfo());
                var definitionStt = (DefinitionStatement) stt.First();

                var transpiler = context.GetEvaluationTranspilerForStatement(definitionStt.DefaultValue);
                var (dataType, exp, template) = transpiler.GetExpression(context, context.GeneralScope, metaWriter,
                    codeWriter, definitionStt, definitionStt.DefaultValue);

                Assert.AreEqual("true", exp);
            }
        }
    }
}
