using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.Parsing;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class UserClassCompilationTests
    {
        [TestMethod]
        public void CompilesSimpleClassAndObjectCreation()
        {
            Platforms.AddPlatform(new UnixBashPlatform());

            const string source = @"
class Point {
    int x;
    void Point(int ax) { this.x = ax; }
}
Point p = new Point(5);
";

            using var reader = new StringReader(source);
            using var metaWriter = new StringWriter();
            using var codeWriter = new StringWriter();

            var context = Helper.CreateBashContext();
            var parser = new Parser(context);
            var statements = parser.Parse(reader, Helper.CreateParserInfo()).ToList();

            Assert.AreEqual(2, statements.Count);
            Assert.IsInstanceOfType(statements[0], typeof(ClassDeclarationStatement));

            foreach (var statement in statements)
            {
                Compiler.Transpile(context, context.GeneralScope, statement, codeWriter, metaWriter);
            }

            var code = codeWriter.ToString();
            StringAssert.Contains(code, "function Point__new()");
            StringAssert.Contains(code, "declare -A p");
            StringAssert.Contains(code, "Point__new p");
        }
    }
}
