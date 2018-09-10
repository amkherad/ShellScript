using System.Collections.Generic;
using System.IO;

namespace ShellScript.CommandLine
{
    public class DaemonCommand : ICommand
    {
        public string Name => "Daemon";
        
        public Dictionary<string, string> SwitchesHelp { get; }
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("--daemon");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            outputWriter.WriteLine("Daemon started.");
            
            return Program.Successful;
        }
    }
}