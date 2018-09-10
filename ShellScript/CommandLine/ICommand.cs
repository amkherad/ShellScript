using System.Collections.Generic;
using System.IO;

namespace ShellScript.CommandLine
{
    public interface ICommand
    {
        string Name { get; }
        
        Dictionary<string, string> SwitchesHelp { get; }

        bool CanHandle(CommandContext command);

        int Execute(
            TextWriter outputWriter,
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter, 
            CommandContext context
        );
    }
}