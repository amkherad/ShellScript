using System;
using System.Collections.Generic;
using System.Reflection;
using ShellScript.CommandLine;

namespace ShellScript.Core
{
    public class ApplicationContext
    {
        public const string Url = "https://github.com/amkherad/ShellScript";
        
        public static ICommand HelpCommand { get; } = new HelpCommand();
        
        public static ICollection<ICommand> AvailableCommands { get; } = new List<ICommand>
        {
            HelpCommand,
            new CompileCommand(),
            new PlatformsCommand(),
            new ExecuteCommand(),
            new VersionInfoCommand(),
            new DaemonCommand(),
        };
        
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}