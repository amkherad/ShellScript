using System;
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

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, CommandContext context)
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

            var result = compiler.CompileFromSource(inputFile, outputFile, platform, true);

            if (result.Successful)
            {
                outputWriter.WriteLine("Compilation finished successfully.");
                return Program.Successful;
            }
            else
            {
                if (context.AnySwitch("--verbose"))
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
    }
}