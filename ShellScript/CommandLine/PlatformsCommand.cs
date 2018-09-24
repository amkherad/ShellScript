using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Language;

namespace ShellScript.CommandLine
{
    public class PlatformsCommand : ICommand
    {
        public string Name => "Platforms";
        
        public Dictionary<string, string> SwitchesHelp { get; }
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("--platforms");
        }

        public ResultCodes Execute(TextWriter outputWriter, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter, CommandContext context)
        {
            foreach (var platform in Platforms.AvailablePlatforms)
            {
                outputWriter.WriteLine(platform.Name);
            }
            
            return ResultCodes.Successful;
        }
    }
}