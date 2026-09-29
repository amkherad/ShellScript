using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.PostProcessing;
using ShellScript.Unix.Bash;
using ShellScript.Unix.Bash.PostProcessing;

namespace ShellScript.MSTest.CompilingTests
{
    [TestClass]
    public class GeneratedCodePostProcessorTests
    {
        [TestMethod]
        public void Pipeline_skips_when_post_process_disabled()
        {
            var flags = CompilerFlags.CreateDefault();
            flags.PostProcessGeneratedCode = false;
            flags.FormatGeneratedCode = true;

            var platform = new UnixBashPlatform();
            var input = "echo hi\n";

            var output = GeneratedCodePostProcessorPipeline.Apply(
                input, platform, flags, "/tmp/out.bash", null, null);

            Assert.AreEqual(input, output);
        }

        [TestMethod]
        public void Bash_format_processor_skips_when_format_flag_off()
        {
            var processor = new FormatBashGeneratedCodePostProcessor();
            var flags = CompilerFlags.CreateDefault();
            flags.FormatGeneratedCode = false;

            Assert.IsFalse(processor.IsEnabled(flags, new UnixBashPlatform()));
        }

        [TestMethod]
        public void Bash_format_processor_indents_control_structures()
        {
            var processor = new FormatBashGeneratedCodePostProcessor();
            var flags = CompilerFlags.CreateDefault();
            flags.FormatGeneratedCode = true;
            flags.GeneratedCodeIndentColumns = 2;

            var input =
                "while\n" +
                "[ $running -ne 0 ]\n" +
                "do\n" +
                "repaint=0\n" +
                "if [ $firstPaint -ne 0 ]\n" +
                "then\n" +
                "repaint=1\n" +
                "fi\n" +
                "done\n";

            var context = new GeneratedCodePostProcessContext(
                input,
                new UnixBashPlatform(),
                flags,
                "/tmp/x.bash",
                null,
                null);

            var result = processor.Process(context);

            var expected =
                "while\n" +
                "[ $running -ne 0 ]\n" +
                "do\n" +
                "  repaint=0\n" +
                "  if [ $firstPaint -ne 0 ]\n" +
                "  then\n" +
                "    repaint=1\n" +
                "  fi\n" +
                "done\n";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Bash_format_processor_elif_then_does_not_accumulate_depth()
        {
            var processor = new FormatBashGeneratedCodePostProcessor();
            var flags = CompilerFlags.CreateDefault();
            flags.FormatGeneratedCode = true;
            flags.GeneratedCodeIndentColumns = 2;

            var input =
                "if [ $a -ne 0 ]\n" +
                "then\n" +
                "x=1\n" +
                "elif [ $b -ne 0 ]\n" +
                "then\n" +
                "x=2\n" +
                "else\n" +
                "x=3\n" +
                "fi\n" +
                "y=0\n";

            var result = processor.Process(new GeneratedCodePostProcessContext(
                input, new UnixBashPlatform(), flags, "/tmp/x.bash", null, null));

            StringAssert.Contains(result, "fi\ny=0");
            Assert.IsFalse(result.Contains("fi\n  y=0"), "code after fi should return to outer indent");
        }

        [TestMethod]
        public void Bash_format_processor_inline_if_fi_on_one_line_is_balanced()
        {
            var processor = new FormatBashGeneratedCodePostProcessor();
            var flags = CompilerFlags.CreateDefault();
            flags.FormatGeneratedCode = true;
            flags.GeneratedCodeIndentColumns = 2;

            var input =
                "function Demo_Fn() {\n" +
                "if [ -n \"$x\" ]; then echo ok; fi\n" +
                "echo tail\n" +
                "}\n";

            var result = processor.Process(new GeneratedCodePostProcessContext(
                input, new UnixBashPlatform(), flags, "/tmp/x.bash", null, null));

            StringAssert.Contains(result, "  echo tail\n}");
        }

        [TestMethod]
        public void Bash_format_processor_indents_function_body()
        {
            var processor = new FormatBashGeneratedCodePostProcessor();
            var flags = CompilerFlags.CreateDefault();
            flags.FormatGeneratedCode = true;
            flags.GeneratedCodeIndentColumns = 2;

            var input =
                "function Demo_Fn() {\n" +
                "local x=1\n" +
                "if [ \"$x\" = \"1\" ]; then\n" +
                "echo ok\n" +
                "fi\n" +
                "}\n";

            var context = new GeneratedCodePostProcessContext(
                input,
                new UnixBashPlatform(),
                flags,
                "/tmp/x.bash",
                null,
                null);

            var result = processor.Process(context);

            StringAssert.Contains(result, "function Demo_Fn() {\n  local x=1");
            StringAssert.Contains(result, "  if [ \"$x\" = \"1\" ]; then\n    echo ok");
            StringAssert.Contains(result, "  fi\n}");
        }
    }
}
