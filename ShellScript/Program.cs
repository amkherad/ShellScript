using System;
using ShellScript.CommandLine;
using ShellScript.Core;
using ShellScript.Core.Language;

namespace ShellScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandContext = CommandContext.Parse(args);

            var outputWriter = Console.Out; //new StreamWriter(Console.OpenStandardError());
            var errorWriter = Console.Out; //new StreamWriter(Console.OpenStandardError());

            foreach (var command in ApplicationContext.AvailableCommands)
            {
                if (command.CanHandle(commandContext))
                {
                    command.Execute(outputWriter, errorWriter, commandContext);
                }
            }

            errorWriter.WriteLine(DesignGuides.ErrorOutputHead + " Invalid command-line passed.");
        }
    }
}