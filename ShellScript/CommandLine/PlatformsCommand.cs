using System.IO;
using ShellScript.Core;
using ShellScript.Core.Language;

namespace ShellScript.CommandLine
{
    public class PlatformsCommand : ICommand
    {
        public string Name => "Platforms";
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("--platforms");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, CommandContext context)
        {
            foreach (var platform in Platforms.AvailablePlatforms)
            {
                outputWriter.WriteLine(platform.Name);
            }
            
            return Program.Successful;
        }
    }
}