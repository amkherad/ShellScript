using System.IO;

namespace ShellScript.CommandLine
{
    public class DaemonCommand : ICommand
    {
        public string Name => "Deamon";
        
        public bool CanHandle(CommandContext command)
        {
            return command.IsCommand("--daemon");
        }

        public int Execute(TextWriter outputWriter, TextWriter errorWriter, CommandContext context)
        {
            outputWriter.WriteLine("Daemon started.");
            
            return Program.Successful;
        }
    }
}