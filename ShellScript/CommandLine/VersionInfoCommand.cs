using System.IO;
using ShellScript.Core;

namespace ShellScript.CommandLine
{
    public class VersionInfoCommand : ICommand
    {
        public string Name => "Version";
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("-v", "--version");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, CommandContext context)
        {
            outputWriter.WriteLine(ApplicationContext.Version);
            
            return Program.Successful;
        }
    }
}