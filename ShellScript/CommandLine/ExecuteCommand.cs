using System.Collections.Generic;
using System.IO;

namespace ShellScript.CommandLine
{
    public class ExecuteCommand : ICommand
    {
        public string Name => "Execute";
        
        public Dictionary<string, string> SwitchesHelp { get; }
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("exec", "--exec");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            errorWriter.WriteLine("Not implemented.");
            return Program.Successful;
        }
    }
}