using System;
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
            var expectedFunctions = new Dictionary<string, string[]>
            {
                {"Convert", new[] {"ToInteger", "ToFloat", "ToNumber", "ToBoolean", "ToString"}},
                {"Environment", new[] {"GetVariable", "GetCurrentDirectory", "GetHomeDirectory"}},
                {"Math", new[] {"Abs"}},
                {"String", new[] {"GetLength", "IsNullOrEmpty", "IsNullOrWhiteSpace", "Contains", "StartsWith", "EndsWith"}},
                {"Array", new[] {"GetLength", "Copy", "Initialize"}},
                {"Platform", new[] {"Call", "CallInteger", "CallFloat", "CallNumeric", "CallString"}},
                {"User", new[] {"IsSuperUser", "GetUserName"}},
                {"File", new[] {"Exists", "CanRead", "CanWrite", "CanExecute", "IsLink", "IsDirectory", "IsFile"}},
                {"Path", new[] {"Combine", "GetFileName", "GetDirectoryName", "GetExtension"}},
                {"Locale", new[] {"GetCurrentLocale"}},
                {"Net", new[] {"Ping"}},
            };

            CollectionAssert.AreEquivalent(expectedFunctions.Keys.ToArray(),
                platform.Api.Classes.Select(apiClass => apiClass.Name).ToArray());

            foreach (var expectedClass in expectedFunctions)
            {
                Assert.IsTrue(platform.Api.TryGetClass(expectedClass.Key, out var apiClass));
                CollectionAssert.AreEquivalent(expectedClass.Value,
                    apiClass.Functions.Select(function => function.Name).ToArray());
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
