using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Unix.Bash;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class IncludeCompilationTests
    {
        [TestInitialize]
        public void Setup()
        {
            if (Platforms.GetPlatformByName(UnixBashPlatform.PlatformName) == null)
            {
                Platforms.AddPlatform(new UnixBashPlatform());
            }
        }

        [TestMethod]
        public void Include_merges_functions_from_sibling_file()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
include ""lib.shellscript"";
echo $""{Double(4)}"";
"),
                ("lib.shellscript", @"
int Double(int n) {
  return n + n;
}
"));

            var bash = Compile(root, "main.shellscript");
            StringAssert.Contains(bash, "function Double()");
            StringAssert.Contains(bash, "4 + 4");
        }

        [TestMethod]
        public void Include_resolves_paths_relative_to_included_file_directory()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
include ""nested/outer.shellscript"";
echo Greet();
"),
                ("nested/outer.shellscript", @"
include ""inner.shellscript"";
string Greet() {
  return Tag() + "" world"";
}
"),
                ("nested/inner.shellscript", @"
string Tag() {
  return ""hello"";
}
"));

            var bash = Compile(root, "main.shellscript");
            StringAssert.Contains(bash, "function Tag()");
            StringAssert.Contains(bash, "function Greet()");
        }

        [TestMethod]
        public void Multiple_includes_share_root_scope()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
include ""a.shellscript"";
include ""b.shellscript"";
echo $""{Add(One(), Two())}"";
"),
                ("a.shellscript", @"
int One() { return 1; }
"),
                ("b.shellscript", @"
int Two() { return 2; }
int Add(int x, int y) { return x + y; }
"));

            var bash = Compile(root, "main.shellscript");
            StringAssert.Contains(bash, "function One()");
            StringAssert.Contains(bash, "function Two()");
            StringAssert.Contains(bash, "function Add()");
        }

        [TestMethod]
        public void Include_preserves_statement_order()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
echo ""before"";
include ""mid.shellscript"";
echo ""after"";
"),
                ("mid.shellscript", @"
echo ""included"";
"));

            var bash = Compile(root, "main.shellscript");
            var before = bash.IndexOf("before", StringComparison.Ordinal);
            var included = bash.IndexOf("included", StringComparison.Ordinal);
            var after = bash.IndexOf("after", StringComparison.Ordinal);
            Assert.IsTrue(before >= 0 && included > before && after > included);
        }

        [TestMethod]
        public void Circular_include_throws_compiler_exception()
        {
            using var root = CreateFixture(
                ("a.shellscript", @"include ""b.shellscript"";"),
                ("b.shellscript", @"include ""a.shellscript"";"));

            var ex = Assert.ThrowsException<CompilerException>(() => Compile(root, "a.shellscript"));
            StringAssert.Contains(ex.Message, "Circular include");
        }

        [TestMethod]
        public void Missing_include_lists_searched_paths()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"include ""missing.shellscript"";"));

            var ex = Assert.ThrowsException<CompilerException>(() => Compile(root, "main.shellscript"));
            StringAssert.Contains(ex.Message, "Include file not found");
            StringAssert.Contains(ex.Message, "missing.shellscript");
        }

        [TestMethod]
        public void Include_with_non_literal_path_fails_validation()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
string path = ""lib.shellscript"";
include path;
"),
                ("lib.shellscript", @"echo ""x"";"));

            var ex = Assert.ThrowsException<CompilerException>(() => Compile(root, "main.shellscript"));
            StringAssert.Contains(ex.Message, "string literal");
        }

        [TestMethod]
        public void Second_include_of_same_file_is_skipped_without_duplicate_symbols()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
include ""config.shellscript"";
include ""use_config.shellscript"";
echo AppName;
"),
                ("config.shellscript", @"
string AppName = ""demo"";
"),
                ("use_config.shellscript", @"
include ""config.shellscript"";
"));

            var bash = Compile(root, "main.shellscript");
            var count = 0;
            var idx = 0;
            while ((idx = bash.IndexOf("AppName=", idx, StringComparison.Ordinal)) >= 0)
            {
                count++;
                idx++;
            }

            Assert.AreEqual(1, count, "config should only be merged once");
        }

        [TestMethod]
        public void Include_inside_function_fails_validation()
        {
            using var root = CreateFixture(
                ("main.shellscript", @"
void Run() {
  include ""lib.shellscript"";
}
"),
                ("lib.shellscript", @"echo ""x"";"));

            var ex = Assert.ThrowsException<CompilerException>(() => Compile(root, "main.shellscript"));
            StringAssert.Contains(ex.Message, "root scope");
        }

        private static string Compile(string root, string entryFile)
        {
            var input = Path.Combine(root, entryFile);
            var output = Path.Combine(root, Path.GetFileNameWithoutExtension(entryFile) + ".bash");
            var obj = Path.Combine(root, "obj");
            Directory.CreateDirectory(obj);

            using (var log = new StringWriter())
            {
                new Compiler().CompileFromSource(input, obj, output, new UnixBashPlatform(),
                    CreateFlags(), log, log, log);
            }

            return File.ReadAllText(output);
        }

        private static CompilerFlags CreateFlags()
        {
            var flags = CompilerFlags.CreateDefault();
            flags.WriteShellScriptVersion = false;
            flags.PreferRandomHelperVariableNames = false;
            return flags;
        }

        private static TempDirectory CreateFixture(params (string relativePath, string content)[] files)
        {
            var root = Path.Combine(Path.GetTempPath(), "ShellScript.IncludeTests." + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            foreach (var (relativePath, content) in files)
            {
                var full = Path.Combine(root, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(full));
                File.WriteAllText(full, content);
            }

            return new TempDirectory(root);
        }

        private sealed class TempDirectory : IDisposable
        {
            public string Path { get; }

            public TempDirectory(string path)
            {
                Path = path;
            }

            public static implicit operator string(TempDirectory d) => d.Path;

            public void Dispose()
            {
                try
                {
                    if (Directory.Exists(Path))
                    {
                        Directory.Delete(Path, true);
                    }
                }
                catch
                {
                    // best-effort cleanup for temp fixtures
                }
            }
        }
    }
}
