using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Core;

namespace ShellScript.CommandLine
{
    public class HelpCommand : ICommand
    {
        public string Name => "Help";

        public Dictionary<string, string> SwitchesHelp { get; }

        public bool CanHandle(CommandContext command)
        {
            if (command.IsCommand("help", "-h", "--help"))
            {
                return true;
            }

            return command.IsEmpty;
        }

        public ResultCodes Execute(TextWriter writer, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            writer.WriteLine($"ShellScript ({ApplicationContext.Version}) by Ali Mousavi Kherad");

            writer.WriteLine();

            WriteEntry(writer, "help, -h, --help", "Will print this help message.");
            WriteEntry(writer, "--platforms", "Shows the installed platforms.");
            WriteEntry(writer, "-v, --version", "Shows the version string.");
            WriteSeperator(writer);
            WriteEntry(writer, "compile", "Compiles the given source/project file.");
            WriteEntry(writer, "exec", "Executes the given source/project file without compilation.");
            WriteEntry(writer, "daemon", "Starts the runtime daemon.");
            WriteEntry(writer, $"--{Program.DuplexErrorOutputSwitchName}", "Writes the error output to both the StdOut and the StdErr.");
            WriteEntry(writer, $"--{Program.CompatibleOutputSwitchName}",
                "Determines that output is used by an external software like an IDE.");

            return ResultCodes.Successful;
        }

        private void WriteEntry(TextWriter writer, string commands, string help)
        {
            const int CommandsWidth = 20;

            while (commands.Split(Environment.NewLine).Any(part => part.Length > CommandsWidth))
            {
                var lastWhiteSpace = commands.LastIndexOf(' ');

                if (lastWhiteSpace > 0)
                {
                    writer.WriteLine("{0," + CommandsWidth + "}", commands.Substring(0, lastWhiteSpace));
                    commands = commands.Substring(lastWhiteSpace + 1);
                }
                else
                {
                    break;
                }
            }

            writer.WriteLine("{0," + CommandsWidth + "}\t{1,-60}", commands, help);
            writer.WriteLine();
        }

        private void WriteSeperator(TextWriter writer)
        {
        }

        public void ShowHelp(TextWriter writer, ICommand command)
        {
            writer.WriteLine($"List of '{command.Name}' command switches:");
            writer.WriteLine("*Switches must start with either '-' or '--'");
            writer.WriteLine();

            if (command.SwitchesHelp != null)
            {
                foreach (var sw in command.SwitchesHelp)
                {
                    WriteEntry(writer, sw.Key, sw.Value);
                }
            }
        }
    }
}