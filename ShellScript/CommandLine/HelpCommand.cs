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

        public int Execute(TextWriter writer, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            writer.WriteLine($"ShellScript ({ApplicationContext.Version}) by Ali Mousavi Kherad");

            writer.WriteLine();

            WriteEntry(writer, "help, -h, --help", "Will print this help message.");
            WriteEntry(writer, "--platforms", "Shows the installed platforms.");
            WriteEntry(writer, "-v, --version", "Shows the version string.");
            WriteSeperator(writer);
            WriteEntry(writer, "compile, -c, --compile", "Compiles the given source/project file.");
            WriteEntry(writer, "exec, --exec", "Executes the given source/project file without compilation.");
            WriteEntry(writer, "--daemon", "Starts the runtime daemon.");

            return Program.Successful;
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
    }
}