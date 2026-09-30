using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class BashApiTests
    {
        [TestMethod]
        public void RegistersCompleteBashApiSurface()
        {
            var platform = new UnixBashPlatform();
            var expectedClassNames = new[]
            {
                "Convert", "Environment", "Math", "String", "StringBuilder", "Array", "Platform", "User",
                "File", "Directory", "Path", "Locale", "Net",
                "OS", "Process", "Thread",
                "Ini", "DotEnv", "Json", "Yaml", "Xml",
                "Text", "Unicode", "DateTime", "Binary", "Regex",
                "Log", "Console", "Cli", "Assert",
            };

            CollectionAssert.AreEquivalent(expectedClassNames,
                platform.Api.Classes.Select(apiClass => apiClass.Name).ToArray());

            foreach (var apiClass in platform.Api.Classes)
            {
                Assert.IsTrue(apiClass.Functions.Length > 0, $"Class {apiClass.Name} has no functions.");
            }
        }

        [TestMethod]
        public void RegistersCoreApiFunctionNames()
        {
            var platform = new UnixBashPlatform();
            var spotChecks = new Dictionary<string, string[]>
            {
                {"Json", new[] {"IsValid", "GetPath", "PrettyPrint"}},
                {"Cli", new[] {"GetArgumentCount", "GetArgument", "HasFlag", "GetFlagValue"}},
                {"Process", new[] {"GetCurrentId", "RunAndCapture"}},
                {"Log", new[] {"Info", "Error"}},
                {"Assert", new[] {"Equals", "True", "Fail"}},
            };

            foreach (var spotCheck in spotChecks)
            {
                Assert.IsTrue(platform.Api.TryGetClass(spotCheck.Key, out var apiClass));
                foreach (var functionName in spotCheck.Value)
                {
                    Assert.IsTrue(apiClass.Functions.Any(f => f.Name == functionName), spotCheck.Key + "." + functionName);
                }
            }
        }

        [TestMethod]
        public void RegistersAllBashLoopTranspilers()
        {
            var statementTypes = new UnixBashPlatform().Transpilers
                .Select(transpiler => transpiler.StatementType)
                .ToArray();

            CollectionAssert.Contains(statementTypes, typeof(WhileStatement));
            CollectionAssert.Contains(statementTypes, typeof(DoWhileStatement));
            CollectionAssert.Contains(statementTypes, typeof(ForStatement));
            CollectionAssert.Contains(statementTypes, typeof(ForEachStatement));
        }
    }
}
