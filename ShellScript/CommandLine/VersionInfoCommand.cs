using System.Collections.Generic;
using System.IO;
using ShellScript.Core;

namespace ShellScript.CommandLine
{
    public class VersionInfoCommand : ICommand
    {
        public string Name => "Version";
        
        public Dictionary<string, string> SwitchesHelp { get; }

        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("-v", "--version");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            outputWriter.WriteLine(ApplicationContext.Version);
            
            return Program.Successful;
        }
    }
}