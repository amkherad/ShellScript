using System.Collections.Generic;
using ShellScript.CommandLine;

namespace ShellScript.Core
{
    public class ApplicationContext
    {        
        public static ICommand HelpCommand { get; } = new HelpCommand();
        
        public static ICollection<ICommand> AvailableCommands { get; } = new List<ICommand>
        {
            HelpCommand
        }; 
    }
}