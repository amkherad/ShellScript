using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Language.CompilerServices;

namespace ShellScript.CommandLine
{
    public class CompileCommand : ICommand
    {
        public string Name => "Compile";

        public bool CanHandle(CommandContext command)
        {
            if (command.IsCommand("compile", "-c", "--compile"))
            {
                return true;
            }

            return false;
        }

        public int Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            CommandContext context)
        {
            var inputFile = context.GetToken(0);
            var outputFile = context.GetToken(1);
            var platform = context.GetToken(2);

            if (inputFile == null)
            {
                errorWriter.WriteLine("Source file is not specified.");
                return Program.Failure;
            }

            if (outputFile == null)
            {
                errorWriter.WriteLine("Output file is not specified.");
                return Program.Failure;
            }

            if (platform == null)
            {
                errorWriter.WriteLine("Platform is not specified.");
                return Program.Failure;
            }

            var compiler = new Compiler();

            var flags = CompilerFlags.CreateDefault();

            _fillCompilerFlags(context, flags);

            var result = compiler.CompileFromSource(
                warningWriter,
                logWriter,
                inputFile,
                outputFile,
                platform,
                flags
            );

            if (result.Successful)
            {
                outputWriter.WriteLine("Compilation finished successfully.");
                return Program.Successful;
            }
            else
            {
                if (context.AnySwitch("verbose"))
                {
                    errorWriter.WriteLine(result.Exception?.ToString());
                }
                else
                {
                    errorWriter.WriteLine(result.Exception?.Message);
                }

                return Program.Failure;
            }
        }

        private void _fillCompilerFlags(CommandContext context, CompilerFlags flags)
        {
            Switch s;

            if ((s = context.GetSwitch(CompilerFlags.ExplicitEchoStreamSwitch)) != null)
            {
                s.AssertValue();
                flags.ExplicitEchoStream = s.Value;
            }

            if ((s = context.GetSwitch(CompilerFlags.DefaultExplicitEchoStreamSwitch)) != null)
            {
                s.AssertValue();
                flags.DefaultExplicitEchoStream = s.Value;
            }
        }
        
        public Dictionary<string, string> SwitchesHelp { get; } = new Dictionary<string, string>
        {
            {CompilerFlags.ExplicitEchoStreamSwitch, ""},
            {"", ""}
        };
    }
}